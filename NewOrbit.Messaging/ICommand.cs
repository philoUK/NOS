using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging
{
    public interface ICommand : IMessage
    {
        string Id { get; }
    }
}
