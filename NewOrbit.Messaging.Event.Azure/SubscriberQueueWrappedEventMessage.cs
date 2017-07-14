using System;
using System.Collections.Generic;
using System.Text;

namespace NewOrbit.Messaging.Event.Azure
{
    public class SubscriberQueueWrappedEventMessage
    {
        public DateTime Date { get; set; }
        public string EventType { get; set; }
        public string EventJson { get; set; }
        public string EventId { get; set; }
        public string SubscriberType { get; set; }

    }
}
