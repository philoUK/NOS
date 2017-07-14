using System;

namespace NewOrbit.Messaging.Shared
{
    public interface IAzureStorageQueueConfig
    {
        string ConnectionString { get; set; }
        string CommandQueue(Type commandType);
        string EventQueue(Type eventType);
        string EventSubscriberQueue(Type eventType);
    }
}