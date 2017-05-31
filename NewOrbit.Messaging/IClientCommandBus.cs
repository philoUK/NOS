using System.Threading.Tasks;

namespace NewOrbit.Messaging
{
    public interface IClientCommandBus
    {
        Task Submit(ICommand command);
    }
}
