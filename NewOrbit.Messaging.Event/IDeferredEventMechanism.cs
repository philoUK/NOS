using System;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Event
{
    public interface IDeferredEventMechanism
    {
        Task Defer(IEvent @event);
        Task DeferToSubscriber(IEvent @event, Type subscriberType);
    }
}