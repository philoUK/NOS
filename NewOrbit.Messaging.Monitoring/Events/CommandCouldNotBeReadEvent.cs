namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandCouldNotBeReadEvent : IEvent
    {
        public string CommandId { get; set; }
        public string CommandType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
