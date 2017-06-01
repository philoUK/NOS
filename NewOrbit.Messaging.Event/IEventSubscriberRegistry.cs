using System;
using System.Collections.Generic;

namespace NewOrbit.Messaging.Event
{
    public interface IEventSubscriberRegistry
    {
        IEnumerable<Type> GetSubscribers(IEvent @event);
    }
}