using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Timeouts;

namespace NewOrbit.Messaging.Dispatch
{
    class SagaTimeoutDispatcher : SagaMessageDispatcher<TimeoutData>
    {

        public SagaTimeoutDispatcher(TimeoutData timeout, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
            :base(timeout, timeout.ExtractTargetType(), sagaDatabase, commandBus, eventBus)
        {

        }

        protected override void DispatchMessage()
        {
            this.Saga.HandleTimeout(this.Message.TargetMethod);
        }
    }
}