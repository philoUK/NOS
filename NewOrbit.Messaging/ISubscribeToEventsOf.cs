namespace NewOrbit.Messaging
{
    public interface ISubscribeToEventsOf<T> where T: IEvent
    {
        void HandleEvent(T @event);
    }
}
