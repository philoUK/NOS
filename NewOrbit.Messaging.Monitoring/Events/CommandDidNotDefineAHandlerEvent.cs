using System;

namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandDidNotDefineAHandlerEvent : IEvent
    {
        public CommandDidNotDefineAHandlerEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string CommandId { get; set; }
        public string CommandType { get; set; }
        public string Id { get; set; }
    }

}
