using System;
using System.ComponentModel;
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
        public static bool HandledEvent = false;

        public void HandleEvent(CommandTestedEvent @event)
        {
            HandledEvent = true;
        }
    }

    public class CommandTestedEventPublisher : IPublishEventsOf<CommandTestedEvent>
    {
        
    }
}
