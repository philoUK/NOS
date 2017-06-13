namespace NewOrbit.Messaging.Saga
{
    public interface ISaga
    {
        string SagaId { get; }
        ISagaData SagaData { get; }
        void Initialise();
        void Load(ISagaData data);
    }
}
