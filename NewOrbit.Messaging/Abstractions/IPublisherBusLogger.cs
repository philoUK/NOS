namespace NewOrbit.Messaging.Abstractions
{
    public interface IPublisherBusLogger
    {
        void LogNoPublisherFoundException(IEvent @event);
        void LogTooManyPublishersFoundException(IEvent @event);
        void LogUnregisteredPublisherException(object publisher, IEvent @event);
    }
}