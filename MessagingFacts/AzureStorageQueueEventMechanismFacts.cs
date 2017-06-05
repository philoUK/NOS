using System;
using System.Threading.Tasks;
using MessagingFacts.Messages;
using Microsoft.WindowsAzure.Storage;
using Moq;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts
{
    public class AzureStorageQueueEventMechanismFacts
    {
        [Fact]
        public async Task TheOutboundMessageIsConstructedProperly()
        {
            var sut = new AzureStorageQueueEventMechanismInterceptor();
            var @event = new CommandTestedEvent();
            await sut.Defer(@event, this.GetType());
            var queuedMessage = sut.Message;
            Assert.NotNull(queuedMessage);
            Assert.Equal(@event.Id, queuedMessage.EventId);
            Assert.Equal(@event.GetType().AssemblyQualifiedName, queuedMessage.EventType);
            Assert.Equal(@event.ToJson(), queuedMessage.EventJson);
            Assert.Equal(this.GetType().AssemblyQualifiedName, queuedMessage.SubscribingType);
        }

        [Fact]
        public async Task UsesSpecifiedMessageQueue()
        {
            var queueName = "azure-eventmechanism-tests";
            var sut = new AzureStorageQueueEventMechanism(AzureStorageQueueEventMechanismInterceptor.GetConfig());
            await sut.Defer(new CommandTestedEvent(), this.GetType());
            Assert.True(await this.QueueExists(queueName));
            await this.DeleteQueue(queueName);
        }

        private async Task<bool> QueueExists(string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            return await queue.ExistsAsync().ConfigureAwait(false);
        }

        private async Task DeleteQueue(string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            await queue.DeleteIfExistsAsync().ConfigureAwait(false);
        }

    }

    internal class AzureStorageQueueEventMechanismInterceptor : AzureStorageQueueEventMechanism
    {
        public static IAzureStorageQueueConfig GetConfig()
        {
            var queueName = "azure-eventmechanism-tests";
            var results = new Mock<IAzureStorageQueueConfig>();
            results.SetupProperty(c => c.ConnectionString, "UseDevelopmentStorage=true");
            results.Setup(c => c.EventQueue(It.Is<Type>(arg => arg == typeof(CommandTestedEvent))))
                .Returns(queueName);
            return results.Object;
        }
        public AzureStorageQueueEventMechanismInterceptor()
            :base(GetConfig())
        {
            
        }

        protected override Task QueueMessage(QueueWrappedEventMessage message)
        {
            this.Message = message;
            return Task.CompletedTask;
        }

        public QueueWrappedEventMessage Message { get; set; }
    }
}
