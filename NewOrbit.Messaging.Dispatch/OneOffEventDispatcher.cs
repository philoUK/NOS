using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    internal class OneOffEventDispatcher
    {
        private readonly IEvent @event;
        private readonly Type subscriberType;
        private readonly IDependencyFactory dependencyFactory;

        public OneOffEventDispatcher(IEvent @event, Type subscriberType, IDependencyFactory dependencyFactory)
        {
            this.@event = @event;
            this.subscriberType = subscriberType;
            this.dependencyFactory = dependencyFactory;
        }

        public void Dispatch()
        {
            var handler = this.dependencyFactory.Make(this.subscriberType);
            var i = handler.GetGenericInterface(typeof(ISubscribeToEventsOf<>),
                this.@event.GetType());
            var method = i.GetMethod("HandleEvent");
            method.Invoke(handler, new object[] {this.@event});
        }
    }
}