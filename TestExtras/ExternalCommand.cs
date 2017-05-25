using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;

namespace TestExtras
{
    public class ExternalCommand : ICommand
    {
    }

    public class ExternalCommandHandler : IHandleCommandsOf<ExternalCommand>
    {
        
    }

    public class BadCommand : ICommand
    {
    }

    public class BadCommandHandler1 : IHandleCommandsOf<BadCommand>
    {
    }

    public class BadCommandHandler2 : IHandleCommandsOf<BadCommand>
    {
    }
}
