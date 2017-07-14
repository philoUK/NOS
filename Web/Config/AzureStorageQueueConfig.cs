using System;
using Microsoft.Extensions.Configuration;
using NewOrbit.Messaging.Shared;

namespace Web.Config
{
    public class AzureStorageQueueConfig : IAzureStorageQueueConfig
    {
        private readonly string commandQueueName;
        private readonly string eventQueueName;
        private readonly string eventDispatchQueue;

        public AzureStorageQueueConfig(IConfigurationRoot configurationRoot)
        {
            var section = configurationRoot.GetSection("queues");
            this.ConnectionString = section["connectionString"];
            this.commandQueueName = section["genericCommandQueue"];
            this.eventQueueName = section["genericEventQueue"];
            this.eventDispatchQueue = section["genericEventDispatchQueue"];
        }

        public string ConnectionString { get; set; }

        public string CommandQueue(Type commandType)
        {
            return this.commandQueueName;
        }

        public string EventQueue(Type eventType)
        {
            return this.eventQueueName;
        }

        public string EventSubscriberQueue(Type eventType)
        {
            return this.eventDispatchQueue;
        }
    }
}
