using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;

namespace NewOrbit.Messaging.Dispatch
{
    internal class SagaCommandDispatcher
    {
        private readonly ICommand command;
        private readonly Type handlerType;
        private readonly DelayedClientCommandBus delayedBus = new DelayedClientCommandBus();
        private readonly DelayedEventBus delayedEventBus = new DelayedEventBus();
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;
        private ISaga saga;

        public SagaCommandDispatcher(ICommand command, Type handlerType, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.command = command;
            this.handlerType = handlerType;
            this.sagaDatabase = sagaDatabase;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task Dispatch()
        {
            this.CreateSaga();
            await this.CreateOrLoadSagaData().ConfigureAwait(false);
            this.DispatchCommand();
            await this.SaveSaga().ConfigureAwait(false);
            await this.SubmitPendingCommands().ConfigureAwait(false);
            await this.SubmitPendingEvents().ConfigureAwait(false);
        }

        private void CreateSaga()
        {
            this.saga = (ISaga)Activator.CreateInstance(this.handlerType,
                delayedBus, delayedEventBus);
        }

        private async Task CreateOrLoadSagaData()
        {
            var exists = await this.sagaDatabase.SagaExists(this.command.CorrelationId)
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
            var sagaData = await this.sagaDatabase.LoadSagaData(this.command.CorrelationId)
                .ConfigureAwait(false);
            this.saga.Load(sagaData);
        }

        private async Task CreateSagaData()
        {
            this.saga.Initialise(this.command.CorrelationId);
            await this.sagaDatabase.Save(this.saga).ConfigureAwait(false);
        }

        private void DispatchCommand()
        {
            this.saga.HandleCommand(this.command);
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
