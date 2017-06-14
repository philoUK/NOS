using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;
using NewOrbit.Messaging.Saga;

namespace NewOrbit.Messaging.Dispatch
{
    public class EventDispatcher
    {
        private readonly IEvent @event;
        private readonly Type subscriberType;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public EventDispatcher(IEvent @event, Type subscriberType, 
            IDependencyFactory dependencyFactory, ISagaDatabase sagaDatabase, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.@event = @event;
            this.subscriberType = subscriberType;
            this.dependencyFactory = dependencyFactory;
            this.sagaDatabase = sagaDatabase;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task Dispatch()
        {
            if (this.subscriberType.IsASaga())
            {
                await new SagaEventDispatcher(this.@event, this.subscriberType, 
                    this.sagaDatabase, this.commandBus, this.eventBus).Dispatch()
                    .ConfigureAwait(false);
            }
            else
            {
                new OneOffEventDispatcher(@event, subscriberType, dependencyFactory)
                    .Dispatch();
            }
        }

    }
}
