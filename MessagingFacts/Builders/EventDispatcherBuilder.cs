using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
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
    }
}
