namespace NewOrbit.Messaging.Event
{
    public interface ILogEventBusMessages
    {
        void NoSubscribersFoundForEvent(IEvent @event);
    }
}