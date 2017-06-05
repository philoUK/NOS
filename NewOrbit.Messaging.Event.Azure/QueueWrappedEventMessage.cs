using System;

namespace NewOrbit.Messaging.Event.Azure
{
    public class QueueWrappedEventMessage
    {
        public DateTime Date { get; set; }
        public string EventType { get; set; }
        public string EventJson { get; set; }
        public string EventId { get; set; }
        public string SubscribingType { get; set; }
    }
}
