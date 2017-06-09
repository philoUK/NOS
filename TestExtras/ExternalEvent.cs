using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalEvent : IEvent
    {
        public string CorrelationId { get; set; }
        public string Id => Guid.NewGuid().ToString();
    }

    public class ExternalEventSubscriber : ISubscribeToEventsOf<ExternalEvent>
    {
        public void HandleEvent(ExternalEvent @event)
        {
        }
    }

    public class ExternalEventPublisher : IPublishEventsOf<ExternalEvent>
    {
        
    }
}
