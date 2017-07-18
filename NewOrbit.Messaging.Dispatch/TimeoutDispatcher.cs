using System.Threading.Tasks;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using NewOrbit.Messaging.Timeouts;
using System;

namespace NewOrbit.Messaging.Dispatch
{
    public class TimeoutDispatcher
    {
        private readonly TimeoutData timeout;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public TimeoutDispatcher(TimeoutData timeout, IDependencyFactory dependencyFactory, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.timeout = timeout;
            this.dependencyFactory = dependencyFactory;
            this.sagaDatabase = sagaDatabase;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task Dispatch()
        {
            var targetType = Type.GetType(this.timeout.TargetType);
            if (targetType.IsASaga())
            {
                await new SagaTimeoutDispatcher(this.timeout, this.sagaDatabase, this.commandBus, this.eventBus)
                    .Dispatch().ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Non Saga types cannot receive timeout messages");
            }
        }
    }
}
