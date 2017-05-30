namespace NewOrbit.Messaging.Monitoring.Events
{
    public class CommandWasQueuedEvent : IEvent
    {
        public string CommandName { get; set; }
    }
}
