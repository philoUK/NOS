using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    public class OneOffEvent : IEvent
    {
        public OneOffEvent()
        {
            this.Id = this.CorrelationId = Guid.NewGuid().ToString();
        }
        public string CorrelationId { get; set; }
        public string Id { get; set; }
    }

    public class OneOffEventHandler : ISubscribeToEventsOf<OneOffEvent>
    {
        public static bool EventHandled;

        public void HandleEvent(OneOffEvent @event)
        {
            EventHandled = true;
        }
    }

    public class OneOffCommand : ICommand
    {
        public OneOffCommand()
        {
            this.Id = this.CorrelationId = Guid.NewGuid().ToString();
        }
        public string CorrelationId { get; set; }
        public string Id { get; set;  }
    }

    public class OneOffCommandHandler : IHandleCommandsOf<OneOffCommand>
    {
        public static bool CommandHandled;

        public void HandleCommand(OneOffCommand command)
        {
            CommandHandled = true;
        }
    }
}
