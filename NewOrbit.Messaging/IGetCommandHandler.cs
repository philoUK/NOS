using System.Collections;
using System.Collections.Generic;

namespace NewOrbit.Messaging
{
    public interface IGetCommandHandler
    {
        IEnumerable<IInvokeCommands> GetCommands(ICommand cmd);
    }
}
