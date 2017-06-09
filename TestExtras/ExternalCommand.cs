using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalCommand : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id => Guid.NewGuid().ToString();
    }

    public class ExternalCommandHandler : IHandleCommandsOf<ExternalCommand>
    {
        public void HandleCommand(ExternalCommand command)
        {
            
        }
    }

    public class BadCommand : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id => Guid.NewGuid().ToString();
    }

    public class BadCommandHandler1 : IHandleCommandsOf<BadCommand>
    {
        public void HandleCommand(BadCommand command)
        {
            
        }
    }

    public class BadCommandHandler2 : IHandleCommandsOf<BadCommand>
    {
        public void HandleCommand(BadCommand command)
        {
            
        }
    }

    public class BadEvent : IEvent
    {
        public string CorrelationId { get; set; }
        public string Id => Guid.NewGuid().ToString();
    }

    public class BadEventPublisher1 : IPublishEventsOf<BadEvent>
    {
    }

    public class BadEventPublisher2 : IPublishEventsOf<BadEvent>
    {
        
    }
}
