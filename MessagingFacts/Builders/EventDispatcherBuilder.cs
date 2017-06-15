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
    internal class EventDispatcherBuilder
    {
        private IEvent @event;
        private Type subscribingType;
        private OneOffSagaData sagaData;
        private readonly Mock<ISagaDatabase> sagaDb = new Mock<ISagaDatabase>();
        private readonly Mock<IClientCommandBus> commandBus = new Mock<IClientCommandBus>();
        private readonly Mock<IEventBus> eventBus = new Mock<IEventBus>();
        private readonly IDependencyFactory factory = new FakeHandlerFactory();

        public EventDispatcherBuilder GivenOneOffEventAndSubscriber()
        {
            OneOffEventHandler.EventHandled = false;
            this.@event = new OneOffEvent();
            this.subscribingType = typeof(OneOffEventHandler);
            return this;
        }

        public async Task<EventDispatcherBuilder> Execute()
        {
            var sut = new EventDispatcher(this.@event, this.subscribingType,
                this.factory, this.sagaDb.Object, this.commandBus.Object,
                this.eventBus.Object);
            await sut.Dispatch().ConfigureAwait(false);
            return this;
        }

        public void AssertSagaPathWasNotTaken()
        {
            this.sagaDb.Verify(db => db.LoadSagaData(It.IsAny<string>()), Times.Never());
            this.sagaDb.Verify(db => db.LoadSagaData(It.IsAny<string>()), Times.Never());
            this.sagaDb.Verify(db => db.Save(It.IsAny<ISaga>()), Times.Never());
        }

        public void AssertEventHandled()
        {
            Assert.True(OneOffEventHandler.EventHandled);
        }

        public EventDispatcherBuilder GivenOneOffEventAndNewSagaSubscriber()
        {
            OneOffEventHandler.EventHandled = false;
            this.@event = new OneOffEvent();
            this.subscribingType = typeof(OneOffSaga);
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

        public EventDispatcherBuilder GivenOneOffEventAndExistingSagaSubscriber()
        {
            OneOffEventHandler.EventHandled = false;
            this.sagaData = new OneOffSagaData {Id = Guid.NewGuid().ToString()};
            this.@event = new OneOffEvent {CorrelationId = this.sagaData.Id};
            this.subscribingType = typeof(OneOffSaga);
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
