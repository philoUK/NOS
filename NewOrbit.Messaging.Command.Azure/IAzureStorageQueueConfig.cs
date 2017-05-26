namespace NewOrbit.Messaging.Command.Azure
{
    public interface IAzureStorageQueueConfig
    {
        string ConnectionString { get; set; }
        string CommandQueue { get; set; }
    }
}