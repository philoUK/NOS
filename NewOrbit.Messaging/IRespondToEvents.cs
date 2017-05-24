namespace NewOrbit.Messaging
{
    public interface IRespondToEvents
    {
        void Respond(IEvent @event);
    }
}