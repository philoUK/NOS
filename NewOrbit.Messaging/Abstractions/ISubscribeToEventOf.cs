namespace NewOrbit.Messaging.Abstractions
{
    public interface ISubscribeToEventOf<in T> : IRespondToEvents where T: IEvent
    {
        void HandleEvent(T @event);
    }
}
