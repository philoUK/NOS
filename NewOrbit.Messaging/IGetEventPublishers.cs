using System;
using System.Collections.Generic;

namespace NewOrbit.Messaging
{
    public interface IGetEventPublishers
    {
        IEnumerable<Type> GetPublishers(IEvent @event);
    }
}