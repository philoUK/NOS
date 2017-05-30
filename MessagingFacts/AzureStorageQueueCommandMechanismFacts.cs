using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using Microsoft.WindowsAzure.Storage;
using Moq;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts
{
    public class AzureStorageQueueCommandMechanismFacts
    {
        private Mock<ICommandHandlerRegistry> CreateRegisty()
        {
            var results = new Mock<ICommandHandlerRegistry>();
            results.Setup(r => r.GetHandlerFor(It.IsAny<TestCommand>()))
                .Returns(typeof(TestCommandHandler));
            return results;
        }

        [Fact]
        public async Task TheOutboundMessageIsConstructedProperly()
        {
            var mock = this.CreateRegisty();
            var sut = new AzureStorageQueueCommandMechanismInterceptor(mock);
            var cmd = new TestCommand();
            await sut.Defer(cmd);
            var queuedMessage = sut.Message;
            Assert.NotNull(queuedMessage);
            Assert.Equal(cmd.GetType().AssemblyQualifiedName, queuedMessage.CommandType);
            Assert.Equal(typeof(TestCommandHandler).AssemblyQualifiedName, queuedMessage.RegisteredHandlerType);
            Assert.Equal(cmd.ToJson(), queuedMessage.CommandJson);
        }

        [Fact]
        public async Task UsesSpecifiedMessageQueue()
        {
            var registry = this.CreateRegisty();
            var queueName = "azure-commandmechanism-tests";
            var config = new Mock<IAzureStorageQueueConfig>();
            config.SetupProperty(c => c.ConnectionString, "UseDevelopmentStorage=true");
            config.Setup(c => c.CommandQueue(It.Is<Type>(arg => arg == typeof(TestCommand))))
                .Returns(queueName);
            var sut = new AzureStorageQueueCommandMechanism(registry.Object, config.Object);
            await sut.Defer(new TestCommand());
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

    internal class AzureStorageQueueCommandMechanismInterceptor : AzureStorageQueueCommandMechanism
    {
        public AzureStorageQueueCommandMechanismInterceptor(Mock<ICommandHandlerRegistry> mock)
            : base(mock.Object, new Mock<IAzureStorageQueueConfig>().Object)
        {
        }

        protected override Task QueueMessage(QueueWrappedMessage msg)
        {
            this.Message = msg;
            return Task.CompletedTask;
        }

        public QueueWrappedMessage Message { get; set; }
    }
}
