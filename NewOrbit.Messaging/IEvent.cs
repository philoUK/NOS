using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging
{
    public interface IEvent : IMessage
    {
        string Id { get; }
    }
}