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
        
    }

    public class BadCommand : ICommand
    {
        public string Id => Guid.NewGuid().ToString();
    }

    public class BadCommandHandler1 : IHandleCommandsOf<BadCommand>
    {
    }

    public class BadCommandHandler2 : IHandleCommandsOf<BadCommand>
    {
    }
}
