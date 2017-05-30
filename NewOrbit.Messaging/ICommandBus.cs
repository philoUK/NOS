using System.Threading.Tasks;

namespace NewOrbit.Messaging
{
    public interface ICommandBus
    {
        Task Submit(ICommand command);
    }
}
