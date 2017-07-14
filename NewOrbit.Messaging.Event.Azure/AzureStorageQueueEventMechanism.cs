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

        public async Task Defer(IEvent @event)
        {
            await this.QueueMessage(this.ConstructEvent(@event)).ConfigureAwait(false);
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

        protected virtual async Task QueueMessage(SubscriberQueueWrappedEventMessage message)
        {
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var eventType = Type.GetType(message.EventType);
            var queue = client.GetQueueReference(this.config.EventSubscriberQueue(eventType));
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
            await queue.AddMessageAsync(new CloudQueueMessage(message.ToJson())).ConfigureAwait(false);
        }

        private QueueWrappedEventMessage ConstructEvent(IEvent @event)
        {
            return new QueueWrappedEventMessage
            {
                Date = DateTime.UtcNow,
                EventId = @event.Id,
                EventType = @event.GetType().AssemblyQualifiedName,
                EventJson = @event.ToJson()
            };
        }

        private SubscriberQueueWrappedEventMessage ConstructEvent(IEvent @event, Type subscriberType)
        {
            return new SubscriberQueueWrappedEventMessage
            {
                Date = DateTime.UtcNow,
                EventId = @event.Id,
                EventType = @event.GetType().AssemblyQualifiedName,
                EventJson = @event.ToJson(),
                SubscriberType = subscriberType.AssemblyQualifiedName
            };
        }


        public async Task DeferToSubscriber(IEvent @event, Type subscriberType)
        {
            await this.QueueMessage(this.ConstructEvent(@event, subscriberType)).ConfigureAwait(false);
        }

    }
}
