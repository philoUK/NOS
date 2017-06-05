using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalCommand : ICommand
    {
        public string Id => Guid.NewGuid().ToString();
    }

    public class ExternalCommandHandler : IHandleCommandsOf<ExternalCommand>
    {
        public void Handle(ExternalCommand command)
        {
            
        }
    }

    public class BadCommand : ICommand
    {
        public string Id => Guid.NewGuid().ToString();
    }

    public class BadCommandHandler1 : IHandleCommandsOf<BadCommand>
    {
        public void Handle(BadCommand command)
        {
            
        }
    }

    public class BadCommandHandler2 : IHandleCommandsOf<BadCommand>
    {
        public void Handle(BadCommand command)
        {
            
        }
    }

    public class BadEvent : IEvent
    {
    }

    public class BadEventPublisher1 : IPublishEventsOf<BadEvent>
    {
    }

    public class BadEventPublisher2 : IPublishEventsOf<BadEvent>
    {
        
    }
}
