namespace NewOrbit.Messaging.Abstractions
{
    public interface IRespondToEvents
    {
        void Respond(IEvent @event);
    }
}