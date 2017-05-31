namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandDidNotDefineAHandlerEvent : IEvent
    {
        public string CommandId { get; set; }
        public string CommandType { get; set; }
    }

}
