using System.Threading.Tasks;

namespace NewOrbit.Messaging
{
    public interface IEventBus
    {
        Task Publish(object publisher, IEvent @event);
    }
}