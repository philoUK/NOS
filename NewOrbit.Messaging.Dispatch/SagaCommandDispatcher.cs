using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;

namespace NewOrbit.Messaging.Dispatch
{
    internal class SagaCommandDispatcher : SagaMessageDispatcher<ICommand>
    {
        public SagaCommandDispatcher(ICommand message, Type handlerType, 
            ISagaDatabase sagaDatabase, IClientCommandBus commandBus, 
            IEventBus eventBus) 
            : base(message, handlerType, sagaDatabase, commandBus, eventBus)
        {
        }

        protected override void DispatchMessage()
        {
            this.Saga.HandleCommand(this.Message);
        }
    }
}
