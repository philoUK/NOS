using System;

namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandWasDispatchedEvent : IEvent
    {
        public CommandWasDispatchedEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string CommandId { get; set; }
        public string CommandType { get; set; }
        public string CorrelationId { get; set; }
        public string Id { get; set; }
    }
}
