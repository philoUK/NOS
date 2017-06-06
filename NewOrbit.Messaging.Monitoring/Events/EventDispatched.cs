using System;

namespace NewOrbit.Messaging.Monitoring.Events
{
    public class EventDispatched : IEvent
    {
        public EventDispatched()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string EventId { get; set; }
        public string EventTypeName { get; set; }

        public string SubscriberTypeName { get; set; }

        public DateTime Date { get; set; }

        public string Id { get; }
    }
}
