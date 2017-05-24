using System.Collections.Generic;

namespace NewOrbit.Messaging
{
    public interface IGetEventSubscribers
    {
        IEnumerable<IRespondToEvents> GetSubscribers(IEvent @event);
    }
}
