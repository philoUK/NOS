namespace NewOrbit.Messaging.Shared
{
    public interface IMessage
    {
        string CorrelationId { get; }
    }
}