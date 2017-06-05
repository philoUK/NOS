using System;

namespace NewOrbit.Messaging.Command.Azure
{
    public class QueueWrappedCommandMessage
    {
        public DateTime Date { get; set; }
        public string CommandType { get; set; }
        public string CommandJson { get; set; }
        public string CommandId { get; set; }
    }
}