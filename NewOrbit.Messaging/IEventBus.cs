using System.Threading.Tasks;

namespace NewOrbit.Messaging
{
    public interface IEventBus
    {
        Task Publish(IEvent @event);
    }
}