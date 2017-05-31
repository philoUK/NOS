using System;

namespace NewOrbit.Messaging.Command.Azure
{
    public class QueueWrappedMessage
    {
        public DateTime Date { get; set; }
        public string CommandType { get; set; }
        public string CommandJson { get; set; }
        public string CommandId { get; set; }
    }
}