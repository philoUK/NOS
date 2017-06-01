using System;
using NewOrbit.Messaging;

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
}
