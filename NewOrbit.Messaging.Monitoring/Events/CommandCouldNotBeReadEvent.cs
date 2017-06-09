using System;

namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandCouldNotBeReadEvent : IEvent
    {
        public CommandCouldNotBeReadEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string CommandId { get; set; }
        public string CommandType { get; set; }
        public string ErrorMessage { get; set; }
        public string CorrelationId { get; set; }
        public string Id { get; set; }
    }
}
