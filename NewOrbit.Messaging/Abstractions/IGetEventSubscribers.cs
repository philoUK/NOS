using System.Collections.Generic;

namespace NewOrbit.Messaging.Abstractions
{
    public interface IGetEventSubscribers
    {
        IEnumerable<IRespondToEvents> GetSubscribers(IEvent @event);
    }
}
