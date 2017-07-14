using System;
using System.Threading.Tasks;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Event;
using TestExtras;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class DeferredEventBusTestBuilder
    {
        private IEvent _event = new CommandTestedEvent();
        private readonly Mock<IEventPublisherRegistry> publisherRegistry = new Mock<IEventPublisherRegistry>();
        private readonly Mock<IDeferredEventMechanism> mechanism = new Mock<IDeferredEventMechanism>();

        public DeferredEventBusTestBuilder WithNoPublishersForEvent<T>() where T: IEvent
        {
            this.publisherRegistry.Setup(r => r.GetPublisher(It.IsAny<IEvent>()))
                .Returns((Type) null);
            return this;
        }

        public async Task<DeferredEventBusTestBuilder> Submit()
        {
            try
            {
                var sut = new DeferredEventBus(this.publisherRegistry.Object, 
                    this.mechanism.Object);
                await sut.Submit(this, this._event).ConfigureAwait(false);
            }
            catch (NoEventPublisherFoundException)
            {
                noPublisherExceptionThrown = true;
            }
            catch (UnauthorizedEventPublisherException)
            {
                this.wrongPublisherExceptionThrown = true;
            }
            return this;
        }

        private bool noPublisherExceptionThrown = false;
        private bool wrongPublisherExceptionThrown;

        public void CheckNoPublisherExceptionThrown()
        {
            Assert.True(noPublisherExceptionThrown);
        }

        public DeferredEventBusTestBuilder WithPublisherForEvent()
        {
            this.publisherRegistry.Setup(p => p.GetPublisher(It.IsAny<IEvent>()))
                .Returns(typeof(DeferredEventBusTestBuilder));
            return this;
        }

        public DeferredEventBusTestBuilder WithIncorrectPublisherForEvent()
        {
            this.publisherRegistry.Setup(r => r.GetPublisher(It.IsAny<IEvent>()))
                .Returns(typeof(BadCommandHandler1));
            return this;
        }

        public void CheckWrongPublisherExceptionThrown()
        {
            Assert.True(this.wrongPublisherExceptionThrown);
        }

        public DeferredEventBusTestBuilder WithMultiplePublishersForEvent()
        {
            return this;
        }

        public void CheckEventWasProperlyQueued()
        {
            this.mechanism.Verify(m => m.Defer(It.IsAny<IEvent>()), Times.Once());
        }
    }
}
