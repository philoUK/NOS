using System.Threading.Tasks;

namespace NewOrbit.Messaging.Command
{
    public interface IDeferredCommandMechanism
    {
        Task Defer(ICommand command);
    }
}