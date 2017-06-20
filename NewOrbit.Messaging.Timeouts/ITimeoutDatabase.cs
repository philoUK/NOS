namespace NewOrbit.Messaging.Timeouts
{
    public interface ITimeoutDatabase
    {
        void Save(TimeoutData timeoutData);
    }
}