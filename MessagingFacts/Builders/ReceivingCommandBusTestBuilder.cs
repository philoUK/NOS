using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using Xunit;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Shared;

namespace MessagingFacts.Builders
{
    internal class ReceivingCommandBusTestBuilder
    {
        private readonly Mock<ICommandHandlerRegistry> registry;
        private readonly Mock<IEventBus> eventBus;
        private string msgJson;
        private readonly TestCommand cmd;

        public ReceivingCommandBusTestBuilder()
        {
            this.registry = new Mock<ICommandHandlerRegistry>();
            this.eventBus = new Mock<IEventBus>();
            this.cmd = new TestCommand();
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
                var sut = new ReceivingCommandBus(registry.Object, eventBus.Object, new FakeHandlerFactory());
                await sut.Submit(this.BuildMessage());
            }
            catch (MessageUnpackingException)
            {
                this.unpackingExceptionThrown = true;
            }
            catch (NoCommandHandlerDefinedException)
            {
                this.noCommandHandlersDefinedExceptionThrown = true;
            }
        }

        private QueueWrappedCommandMessage BuildMessage()
        {
            return new QueueWrappedCommandMessage
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
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<CommandCouldNotBeReadEvent>()), Times.Once());
        }

        public ReceivingCommandBusTestBuilder GivenNoCommandHandlers()
        {
            this.registry.Setup(r => r.GetHandlerFor(It.IsAny<TestCommand>()))
                .Throws(new NoCommandHandlerDefinedException(this.cmd));
            return this;
        }

        private bool noCommandHandlersDefinedExceptionThrown = false;

        public void VerifyNoCommandHandlerDefinedExceptionIsThrown()
        {
            Assert.True(noCommandHandlersDefinedExceptionThrown);
        }

        public void VerifyNoHandlerErrorEventIsRaised()
        {
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<CommandDidNotDefineAHandlerEvent>()), Times.Once());
        }

        public ReceivingCommandBusTestBuilder GivenACommand()
        {
            this.msgJson = cmd.ToJson();
            return this;
        }

        public ReceivingCommandBusTestBuilder GivenACommandHandler()
        {
            this.registry.Setup(r => r.GetHandlerFor(It.IsAny<TestCommand>()))
                .Returns(typeof(TestCommandHandler));
            return this;
        }

        public void VerifyCommandHandledEventIsRaised()
        {
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<CommandWasDispatchedEvent>()), Times.Once());
        }

    }
}
