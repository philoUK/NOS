using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace MessagingFacts.Messages
{
    public class CommandTestedEvent : IEvent 
    {
    }

    public class CommandTestedEventSubscriber : ISubscribeToEventsOf<CommandTestedEvent>
    {
        
    }
}
