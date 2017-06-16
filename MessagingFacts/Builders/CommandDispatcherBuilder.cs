using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class CommandDispatcherBuilder
    {
        private ICommand command;
        private Type handlingType;
        private readonly Mock<ISagaDatabase> sagaDb = new Mock<ISagaDatabase>();
        private readonly Mock<IClientCommandBus> commandBus = new Mock<IClientCommandBus>();
        private readonly Mock<IEventBus> eventBus = new Mock<IEventBus>();
        private readonly IDependencyFactory factory = new FakeHandlerFactory();
        private OneOffSagaData sagaData;

        public CommandDispatcherBuilder GivenOneOffCommandAndHandler()
        {
            OneOffCommandHandler.CommandHandled = false;
            this.handlingType = typeof(OneOffCommandHandler);
            this.command = new OneOffCommand();
            return this;
        }

        public async Task<CommandDispatcherBuilder> Execute()
        {
            var sut = new CommandDispatcher(this.command, this.handlingType, this.factory, this.sagaDb.Object,
                this.commandBus.Object, this.eventBus.Object);
            await sut.Dispatch().ConfigureAwait(false);
            return this;
        }

        public void AssertSagaPathWasNotTaken()
        {
            this.sagaDb.Verify(db => db.LoadSagaData(It.IsAny<string>()), Times.Never());
            this.sagaDb.Verify(db => db.LoadSagaData(It.IsAny<string>()), Times.Never());
            this.sagaDb.Verify(db => db.Save(It.IsAny<ISaga>()), Times.Never());
        }

        public void AssertCommandHandled()
        {
            Assert.True(OneOffCommandHandler.CommandHandled);
        }

        public CommandDispatcherBuilder GivenOneOffCommandAndNewSagaHandler()
        {
            OneOffCommandHandler.CommandHandled = true;
            this.command = new OneOffCommand();
            this.handlingType = typeof(OneOffSaga);
            this.sagaDb.Setup(db => db.SagaExists(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            return this;
        }

        public void AssertNewSagaPathWasTaken()
        {
            this.sagaDb.Verify(db => db.Save(It.IsAny<ISaga>()), Times.Exactly(2));
            this.commandBus.Verify(bus => bus.Submit(It.IsAny<ICommand>()), Times.Once());
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<IEvent>()),
                Times.Once());
        }

        public CommandDispatcherBuilder GivenOneOffCommandAndExistingSagaHandler()
        {
            OneOffCommandHandler.CommandHandled = true;
            this.sagaData = new OneOffSagaData {Id = Guid.NewGuid().ToString()};
            this.command = new OneOffCommand {CorrelationId = this.sagaData.Id};
            this.handlingType = typeof(OneOffSaga);
            this.sagaDb.Setup(db => db.SagaExists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            this.sagaDb.Setup(db => db.LoadSagaData(It.IsAny<string>()))
                .Returns(Task.FromResult((ISagaData)this.sagaData));
            return this;
        }

        public void AssertExistingSagaPathWasTaken()
        {
            this.sagaDb.Verify(db => db.LoadSagaData(It.IsAny<string>()), Times.Once());
            this.sagaDb.Verify(db => db.Save(It.IsAny<ISaga>()), Times.Once());
            this.commandBus.Verify(bus => bus.Submit(It.IsAny<ICommand>()), Times.Once());
            this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<IEvent>()),
                Times.Once());
        }
    }
}
