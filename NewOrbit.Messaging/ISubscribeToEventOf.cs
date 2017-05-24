namespace NewOrbit.Messaging
{
    public interface ISubscribeToEventOf<in T> : IRespondToEvents where T: IEvent
    {
        void HandleEvent(T @event);
    }
}
