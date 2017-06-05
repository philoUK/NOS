using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalEvent : IEvent
    {
        public string Id => Guid.NewGuid().ToString();
    }

    public class ExternalEventSubscriber : ISubscribeToEventsOf<ExternalEvent>
    {
        
    }

    public class ExternalEventPublisher : IPublishEventsOf<ExternalEvent>
    {
        
    }
}
