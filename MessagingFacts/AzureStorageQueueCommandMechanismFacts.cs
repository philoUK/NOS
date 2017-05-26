using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
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
