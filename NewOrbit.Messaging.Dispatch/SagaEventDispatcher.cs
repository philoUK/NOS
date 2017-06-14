using System;
using NewOrbit.Messaging.Saga;

namespace NewOrbit.Messaging.Dispatch
{
    internal class SagaEventDispatcher : SagaMessageDispatcher<IEvent>
    {
        public SagaEventDispatcher(IEvent message, Type handlerType, 
            ISagaDatabase sagaDatabase, IClientCommandBus commandBus, 
            IEventBus eventBus) 
            : base(message, handlerType, sagaDatabase, commandBus, eventBus)
        {
        }

        protected override void DispatchMessage()
        {
            this.Saga.HandleEvent(this.Message);
        }
    }

}