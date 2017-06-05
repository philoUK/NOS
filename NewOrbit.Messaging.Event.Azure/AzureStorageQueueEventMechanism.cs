using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Event.Azure
{
    public class AzureStorageQueueEventMechanism : IDeferredEventMechanism
    {
        private readonly IAzureStorageQueueConfig config;

        public AzureStorageQueueEventMechanism(IAzureStorageQueueConfig config)
        {
            this.config = config;
        }

        public async Task Defer(IEvent @event, Type subscribingType)
        {
            await this.QueueMessage(this.ConstructEvent(@event, subscribingType)).ConfigureAwait(false);
        }

        protected virtual async Task QueueMessage(QueueWrappedEventMessage message)
        {
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var eventType = Type.GetType(message.EventType);
            var queue = client.GetQueueReference(this.config.EventQueue(eventType));
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
            await queue.AddMessageAsync(new CloudQueueMessage(message.ToJson())).ConfigureAwait(false);
        }

        private QueueWrappedEventMessage ConstructEvent(IEvent @event, Type subscribingType)
        {
            return new QueueWrappedEventMessage
            {
                Date = DateTime.UtcNow,
                EventId = @event.Id,
                EventType = @event.GetType().AssemblyQualifiedName,
                EventJson = @event.ToJson(),
                SubscribingType = subscribingType.AssemblyQualifiedName
            };
        }
    }
}
