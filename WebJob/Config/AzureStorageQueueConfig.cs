using System;
using Microsoft.Extensions.Configuration;
using NewOrbit.Messaging.Shared;

namespace WebJob.Config
{
    class AzureStorageQueueConfig : IAzureStorageQueueConfig
    {
        private readonly string commandQueueName;
        private readonly string eventQueueName;
        private readonly string eventDispatchQueue;
        private readonly string timeoutQueueName;

        public AzureStorageQueueConfig(IConfigurationRoot configurationRoot)
        {
            var section = configurationRoot.GetSection("queues");
            this.ConnectionString = section["connectionString"];
            this.commandQueueName = section["genericCommandQueue"];
            this.eventQueueName = section["genericEventQueue"];
            this.eventDispatchQueue = section["genericEventDispatchQueue"];
            this.timeoutQueueName = section["generictTmeoutQueue"];
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