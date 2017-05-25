using System;
using System.Collections.Generic;

namespace NewOrbit.Messaging.Abstractions
{
    public interface IGetEventPublishers
    {
        IEnumerable<Type> GetPublishers(IEvent @event);
    }
}