namespace NewOrbit.Messaging
{
    public interface IPublisherBusLogger
    {
        void LogNoPublisherFoundException(IEvent @event);
        void LogTooManyPublishersFoundException(IEvent isAny);
    }
}