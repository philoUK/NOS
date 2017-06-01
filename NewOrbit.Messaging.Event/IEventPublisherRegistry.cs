using System;

namespace NewOrbit.Messaging.Event
{
    public interface IEventPublisherRegistry
    {
        Type GetPublisher(IEvent @event);
    }
}
