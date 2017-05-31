using System;
using System.Threading.Tasks;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using Xunit;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Monitoring.Events;

namespace MessagingFacts.Builders
{
    class ReceivingCommandBusTestBuilder
    {
        private readonly Mock<ICommandHandlerRegistry> registry;
        private readonly Mock<IEventBus> eventBus;
        private string msgJson;

        public ReceivingCommandBusTestBuilder()
        {
            this.registry = new Mock<ICommandHandlerRegistry>();
            this.eventBus = new Mock<IEventBus>();
        }

        public ReceivingCommandBusTestBuilder GivenAnErrorUnpackingAMessage()
        {
            this.msgJson = "invalid not json";
            return this;
        }

        public async Task Receive()
        {
            try
            {
                var sut = new ReceivingCommandBus(registry.Object, eventBus.Object);
                await sut.Submit(this.BuildMessage());
            }
            catch (MessageUnpackingException)
            {
                this.unpackingExceptionThrown = true;
            }
        }

        private QueueWrappedMessage BuildMessage()
        {
            return new QueueWrappedMessage
            {
                CommandId = "38",
                CommandJson = this.msgJson,
                CommandType = typeof(TestCommand).AssemblyQualifiedName,
                Date = DateTime.Now
            };
        }

        private bool unpackingExceptionThrown = false;

        public void VerifyMessageUnpackingExceptionIsThrown()
        {
            Assert.True(unpackingExceptionThrown);
        }

        public void VerifyUnpackingErrorEventIsRaised()
        {
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<CommandCouldNotBeReadEvent>()), Times.Once());
        }
    }
}
