using System;

namespace NewOrbit.Messaging.Event
{
    [Serializable]
    public class MultipleEventPublishersFoundException : Exception
    {
        public MultipleEventPublishersFoundException(IEvent @event)
            :base($"More than 1 class implements IPublishEventsOf<{@event.GetType().Name}>")
        {
        }

    }
}