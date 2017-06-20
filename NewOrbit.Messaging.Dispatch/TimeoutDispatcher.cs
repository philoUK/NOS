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
            var sagaData = await sagaDatabase.LoadSagaData(timeoutData.TargetId).ConfigureAwait(false);
            var cmdBus = new DelayedClientCommandBus();
            var evtBus = new DelayedEventBus();
            var saga = (ISaga)Activator.CreateInstance(Type.GetType(timeoutData.TargetType), cmdBus, evtBus);
            saga.Load(sagaData);
            var method = saga.GetType().GetMethod(timeoutData.TargetMethod,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method.Invoke(saga, new object[] { });
            await sagaDatabase.Save(saga).ConfigureAwait(false);
            foreach (var cmd in cmdBus.DelayedCommands)
            {
                await this.externalCommandBus.Submit(cmd).ConfigureAwait(false);
            }
            foreach (var @event in evtBus.DelayedEvents)
            {
                await this.externalEventBus.Publish(saga, @event).ConfigureAwait(false);
            }
        }

        private void DeleteTimeout(TimeoutData data)
        {
            database.Delete(data.TargetId, data.TargetMethod);
        }
    }
}
