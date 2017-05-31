namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandWasDispatchedEvent : IEvent
    {
        public string CommandId { get; set; }
        public string CommandType { get; set; }
    }
}
