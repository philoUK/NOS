using System;

namespace NewOrbit.Messaging.Event
{
    [Serializable]
    public class UnauthorizedEventPublisherException : Exception
    {
        public UnauthorizedEventPublisherException(object publisher, IEvent @event)
            :base($"{publisher.GetType().AssemblyQualifiedName} is not the sole inplementor of IPublishEventsOf<{@event.GetType().Name}>")
        {
        }

    }
}