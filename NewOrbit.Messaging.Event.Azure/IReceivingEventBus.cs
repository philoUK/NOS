using System.Threading.Tasks;

namespace NewOrbit.Messaging.Event.Azure
{
    public interface IReceivingEventBus
    {
        Task Dispatch(QueueWrappedEventMessage message);
    }
}