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
}
