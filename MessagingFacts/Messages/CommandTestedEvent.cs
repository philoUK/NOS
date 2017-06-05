using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace MessagingFacts.Messages
{
    public class CommandTestedEvent : IEvent
    {
        public CommandTestedEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id {get;}
    }

    public class CommandTestedEventSubscriber : ISubscribeToEventsOf<CommandTestedEvent>
    {
        
    }

    public class CommandTestedEventPublisher : IPublishEventsOf<CommandTestedEvent>
    {
        
    }
}
