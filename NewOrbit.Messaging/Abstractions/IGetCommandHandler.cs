using System.Collections.Generic;

namespace NewOrbit.Messaging.Abstractions
{
    public interface IGetCommandHandler
    {
        IEnumerable<IInvokeCommands> GetCommands(ICommand cmd);
    }
}
