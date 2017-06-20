namespace NewOrbit.Messaging.Timeouts.Azure
{
    public interface ITimeoutDatabaseConfig
    {
        string ConnectionString { get; set; }
        string TableName { get; set; }
    }
}