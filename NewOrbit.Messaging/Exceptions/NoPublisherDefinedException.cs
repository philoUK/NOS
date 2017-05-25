using System;

namespace NewOrbit.Messaging.Exceptions
{
    public class NoPublisherDefinedException : Exception
    {
        public NoPublisherDefinedException(object publisher, IEvent @event)
            :base($"No publisher found for event {@event.GetType().Name}.  Mark {publisher.GetType().Name} with IPublishEvent<{@event.GetType().Name}>")
        {
        }

    }
}