using System;

namespace NewOrbit.Messaging.Shared
{
    public class QueueWrappedMessage
    {
        public DateTime Date { get; set; }
        public string MessageType { get; set; }
        public string MessageJson { get; set; }
        public string MessageId { get; set; }
    }
}