namespace NewOrbit.Messaging.Saga.Azure
{
    public interface ISagaDatabaseConfig
    {
        string ConnectionString { get; }
        string TableName { get; }
    }
}