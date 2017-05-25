namespace NewOrbit.Messaging.Abstractions
{
    public interface IAmStartedByEventOf<in T> : IRespondToEvents where T: IEvent
    {
        void StartByEvent(T @event);
    }
}
