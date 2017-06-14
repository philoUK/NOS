using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    internal class SagaEventDispatcher
    {
        private readonly IEvent @event;
        private readonly Type subscriberType;
        private readonly DelayedClientCommandBus delayedBus = new DelayedClientCommandBus();
        private readonly DelayedEventBus delayedEventBus = new DelayedEventBus();
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;
        private ISaga saga;

        public SagaEventDispatcher(IEvent @event, Type subscriberType, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.@event = @event;
            this.subscriberType = subscriberType;
            this.sagaDatabase = sagaDatabase;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task Dispatch()
        {
            this.CreateSaga();
            await this.CreateOrLoadSagaData().ConfigureAwait(false);
            this.DispatchEvent();
            await this.SaveSaga().ConfigureAwait(false);
            await this.SubmitPendingCommands().ConfigureAwait(false);
            await this.SubmitPendingEvents().ConfigureAwait(false);
        }

        private void CreateSaga()
        {
            this.saga = (ISaga)Activator.CreateInstance(this.subscriberType,
                delayedBus, delayedEventBus);
        }

        private async Task CreateOrLoadSagaData()
        {
            var exists = await this.sagaDatabase.SagaExists(this.@event.CorrelationId)
                .ConfigureAwait(false);
            if (exists)
            {
                await this.LoadSagaData().ConfigureAwait(false);
            }
            else
            {
                await this.CreateSagaData().ConfigureAwait(false);
            }
        }

        private async Task LoadSagaData()
        {
            var sagaData = await this.sagaDatabase.LoadSagaData(this.@event.CorrelationId)
                .ConfigureAwait(false);
            this.saga.Load(sagaData);
        }

        private async Task CreateSagaData()
        {
            this.saga.Initialise(this.@event.CorrelationId);
            await this.sagaDatabase.Save(this.saga).ConfigureAwait(false);
        }

        private void DispatchEvent()
        {
            var i = this.saga.GetGenericInterface(typeof(ISubscribeToEventsOf<>),
                this.@event.GetType());
            var method = i.GetMethod("HandleEvent");
            method.Invoke(this.saga, new object[] {this.@event});
        }

        private async Task SaveSaga()
        {
            await this.sagaDatabase.Save(this.saga).ConfigureAwait(false);
        }

        private async Task SubmitPendingCommands()
        {
            foreach (var cmd in this.delayedBus.DelayedCommands)
            {
                await this.commandBus.Submit(cmd).ConfigureAwait(false);
            }
        }

        private async Task SubmitPendingEvents()
        {
            foreach (var evt in this.delayedEventBus.DelayedEvents)
            {
                await this.eventBus.Publish(this.saga, evt).ConfigureAwait(false);
            }
        }
    }
}