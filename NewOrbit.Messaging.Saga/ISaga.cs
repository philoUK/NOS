namespace NewOrbit.Messaging.Saga
{
    public interface ISaga
    {
        void Initialise();
        void Load(ISagaData data);
    }
}
