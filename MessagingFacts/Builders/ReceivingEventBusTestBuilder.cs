using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts.Builders
{
    class ReceivingEventBusTestBuilder
    {
        private CommandTestedEvent _event;
        private string eventText;
        private string subscribingType;
        private readonly Mock<IEventBus> eventBus = new Mock<IEventBus>();
        private bool unpackingExceptionThrown = false;

        public ReceivingEventBusTestBuilder()
        {
            this._event = new CommandTestedEvent();
            this.eventText = _event.ToJson();
            this.subscribingType = typeof(CommandTestedEventSubscriber).AssemblyQualifiedName;
        }

        public ReceivingEventBusTestBuilder GivenAnEventWithBadInput()
        {
            CommandTestedEventSubscriber.HandledEvent = false;
            this.eventText = "not json data";
            return this;
        }

        public async Task<ReceivingEventBusTestBuilder> SubmitEvent()
        {
            var sut = new ReceivingEventBus(new FakeHandlerFactory(), this.eventBus.Object);
            try
            {
                await sut.Dispatch(this.MakeMessage()).ConfigureAwait(false);
            }
            catch (MessageUnpackingException)
            {
                this.unpackingExceptionThrown = true;
            }
            return this;
        }

        private QueueWrappedEventMessage MakeMessage()
        {
            return new QueueWrappedEventMessage
            {
                Date = DateTime.UtcNow,
                EventId = _event.Id,
                EventJson = this.eventText,
                EventType = this._event.GetType().AssemblyQualifiedName,
                SubscribingType = this.subscribingType
            };
        }

        public void CheckEventUnpackingExceptionWasThrown()
        {
            Assert.True(this.unpackingExceptionThrown);
        }

        public ReceivingEventBusTestBuilder GivenABadSubscriberType()
        {
            this.subscribingType = "invalid";
            return this;
        }

        public void CheckEventWasDispatched()
        {
            Assert.True(CommandTestedEventSubscriber.HandledEvent);
        }

        public void CheckEventDispatchEventRaised()
        {
            this.eventBus.Verify(
                bus => bus.Publish(It.IsAny<object>(), It.Is<IEvent>(e => e.GetType() == typeof(EventDispatched))),
                Times.Once());
        }
    }
}
