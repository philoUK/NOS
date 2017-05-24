namespace NewOrbit.Messaging
{
    public interface IAmStartedByEventOf<in T> : IRespondToEvents where T: IEvent
    {
        void StartByEvent(T @event);
    }
}
