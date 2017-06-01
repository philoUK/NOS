using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Registrars;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class DeferredEventBusTestBuilder
    {
        private IEvent _event;
        private readonly Mock<IEventPublisherRegistry> publisherRegistry = new Mock<IEventPublisherRegistry>();
        private readonly Mock<ILogEventBusMessages> logger = new Mock<ILogEventBusMessages>();
        private readonly Mock<IEventSubscriberRegistry> subscriberRegistry = new Mock<IEventSubscriberRegistry>();
        private readonly Mock<IDeferredEventMechanism> mechanism = new Mock<IDeferredEventMechanism>();
        public DeferredEventBusTestBuilder WithNoPublishersForEvent<T>() where T: IEvent
        {
            this.publisherRegistry.Setup(r => r.GetPublisher(It.IsAny<IEvent>()))
                .Returns((Type) null);
            return this;
        }

        public DeferredEventBusTestBuilder WithEvent()
        {
            this._event = new CommandTestedEvent();
            return this;
        }

        public async Task<DeferredEventBusTestBuilder> Submit()
        {
            try
            {
                var sut = new DeferredEventBus(this.publisherRegistry.Object, this.logger.Object, this.subscriberRegistry.Object, 
                    this.mechanism.Object);
                await sut.Submit(this._event).ConfigureAwait(false);
            }
            catch (NoEventPublisherFoundException)
            {
                noPublisherExceptionThrown = true;
            }
            return this;
        }

        private bool noPublisherExceptionThrown = false;
        private int subscriberCount;

        public void CheckNoPublisherExceptionThrown()
        {
            Assert.True(noPublisherExceptionThrown);
        }

        public DeferredEventBusTestBuilder WithPublisherForEvent()
        {
            this.publisherRegistry.Setup(p => p.GetPublisher(It.IsAny<IEvent>()))
                .Returns(typeof(EventPublisher));
            return this;
        }

        private class EventPublisher : IPublishEventsOf<CommandTestedEvent>
        {
        }

        public void CheckNoSubscribersWasLogged()
        {
            this.logger.Verify(l => l.NoSubscribersFoundForEvent(It.IsAny<IEvent>()), Times.Once());
        }

        public DeferredEventBusTestBuilder WithMultipleSubscribersToEvent(int subscriberCount)
        {
            this.subscriberCount = subscriberCount;
            this.subscriberRegistry.Setup(r => r.GetSubscribers(It.IsAny<IEvent>()))
                .Returns(GetSubscribers());
            return this;
        }

        private IEnumerable<Type> GetSubscribers()
        {
            for (var i = 0; i < this.subscriberCount; i++)
            {
                yield return this.GetType();
            }
        }

        public void CheckEachMessageWasQueuedUp()
        {
            this.mechanism.Verify(m => m.Defer(It.IsAny<IEvent>(), It.IsAny<Type>()),
                Times.Exactly(this.subscriberCount));
        }
    }
}
