using System;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Shared
{
    public interface IHandlerFactory
    {
        Task<object> Make(Type type, IMessage msg);
    }
}