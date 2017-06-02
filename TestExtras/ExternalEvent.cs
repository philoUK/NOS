using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalEvent : IEvent
    {
    }

    public class ExternalEventSubscriber : ISubscribeToEventsOf<ExternalEvent>
    {
        
    }
}
