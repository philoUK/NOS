using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Timeouts;

namespace NewOrbit.Messaging.Dispatch
{
    public class TimeoutDispatcher
    {
        private readonly ITimeoutDatabase database;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus externalCommandBus;
        private readonly IEventBus externalEventBus;

        private int currentTimeoutPeriodMs = 1000;
        private const int OneMinuteMs = 60000;

        public TimeoutDispatcher(ITimeoutDatabase database, ISagaDatabase sagaDatabase, IEventBus externalEventBus, IClientCommandBus externalCommandBus)
        {
            this.database = database;
            this.sagaDatabase = sagaDatabase;
            this.externalEventBus = externalEventBus;
            this.externalCommandBus = externalCommandBus;
        }

        public async Task Monitor(CancellationToken token = default(CancellationToken))
        {
            while (true)
            {
                var foundData = false;
                foreach (var timeoutData in database.GetExpiredTimeoutsSince(DateTime.UtcNow))
                {
                    foundData = true;
                    await DispatchToSaga(timeoutData, token);
                    DeleteTimeout(timeoutData);
                    token.ThrowIfCancellationRequested();
                    currentTimeoutPeriodMs = 1000;
                }
                token.ThrowIfCancellationRequested();
                await Task.Delay(currentTimeoutPeriodMs, token).ConfigureAwait(false);
                if (!foundData)
                {
                    currentTimeoutPeriodMs += 1000;
                    if (currentTimeoutPeriodMs > OneMinuteMs)
                    {
                        currentTimeoutPeriodMs = 1000;
                    }
                }
            }
        }

        private async Task DispatchToSaga(TimeoutData timeoutData, CancellationToken token)
        {
            var evtBus = new DelayedEventBus();
            var cmdBus = new DelayedClientCommandBus();
            var saga = await this.LoadSaga(timeoutData, cmdBus, evtBus).ConfigureAwait(false);
            saga.HandleTimeout(timeoutData.TargetMethod);
            await sagaDatabase.Save(saga).ConfigureAwait(false);
            await ForwardSagaMessages(cmdBus, evtBus, saga).ConfigureAwait(false);
        }

        private async Task<ISaga> LoadSaga(TimeoutData timeoutData, DelayedClientCommandBus cmdBus, DelayedEventBus evtBus)
        {
            var sagaData = await sagaDatabase.LoadSagaData(timeoutData.TargetId).ConfigureAwait(false);
            var saga = (ISaga) Activator.CreateInstance(Type.GetType(timeoutData.TargetType), cmdBus, evtBus);
            saga.Load(sagaData);
            return saga;
        }

        private void DeleteTimeout(TimeoutData data)
        {
            database.Delete(data.TargetId, data.TargetMethod);
        }

        private async Task ForwardSagaMessages(DelayedClientCommandBus cmdBus, DelayedEventBus evtBus, ISaga saga)
        {
            foreach (var cmd in cmdBus.DelayedCommands)
            {
                await this.externalCommandBus.Submit(cmd).ConfigureAwait(false);
            }
            foreach (var @event in evtBus.DelayedEvents)
            {
                await this.externalEventBus.Publish(saga, @event).ConfigureAwait(false);
            }
        }
    }
}
