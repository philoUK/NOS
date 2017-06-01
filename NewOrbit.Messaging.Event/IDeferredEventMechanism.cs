using System;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Event
{
    public interface IDeferredEventMechanism
    {
        Task Defer(IEvent @event, Type subscribingType);
    }
}