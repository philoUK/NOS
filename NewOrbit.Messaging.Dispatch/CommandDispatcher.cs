using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    public class CommandDispatcher
    {
        private readonly ICommand command;
        private readonly Type handlingType;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public CommandDispatcher(ICommand command, Type handlingType, IDependencyFactory dependencyFactory, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.command = command;
            this.handlingType = handlingType;
            this.dependencyFactory = dependencyFactory;
            this.sagaDatabase = sagaDatabase;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task Dispatch()
        {
            if (this.handlingType.IsASaga())
            {
                await new SagaCommandDispatcher(this.command, this.handlingType,
                        this.sagaDatabase, this.commandBus, this.eventBus)
                    .Dispatch().ConfigureAwait(false);
            }
            else
            {
                new OneOffCommandDispatcher(this.command, this.handlingType,
                    this.dependencyFactory).Dispatch();
            }
        }
    }
}
