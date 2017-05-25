using MessagingFacts.Utilities;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Abstractions;
using Xunit;

namespace MessagingFacts
{
    public class PublisherBusFacts : IPublishEvent<FakeEvent>
    {
        [Fact]
        public void NoPublisherFails()
        {
            var builder = new PublisherBusTester()
                .GivenSubscriber<FakeEvent, FakeEventSubscriberOne>()
                .GivenSubscriber<FakeEvent, FakeEventSubscriberTwo>();
            var @event = new FakeEvent();
            builder.Execute(this, @event);
            builder.AssertNoPublisherFoundErrorThrownAndLogged();
        }

        [Fact]
        public void MoreThanOnePublisherFails()
        {
            var builder = new PublisherBusTester()
                .GivenPublisher<FakeEvent, PublisherBusFacts>()
                .GivenPublisher<FakeEvent, PublisherBusFacts>();
            var @event = new FakeEvent();
            builder.Execute(this, @event);
            builder.AssertTooManyPublishersFoundErrorThrownAndLogged();
        }

        [Fact]
        public void UnRegisteredPublisherFails()
        {
            var builder = new PublisherBusTester()
                .GivenPublisher<FakeEvent, FakeEvent>();
            var @event = new FakeEvent();
            builder.Execute(this, @event);
            builder.AssertUnregisteredPublisherErrorThrownAndLogged();
        }

        [Fact]
        public void SingleAuthorisedPublisherSucceeds()
        {
            var builder = new PublisherBusTester()
                .GivenPublisher<FakeEvent, PublisherBusFacts>()
                .GivenSubscriber<FakeEvent, FakeEventSubscriberOne>();
            var @event = new FakeEvent();
            Assert.False(@event.Published);
            builder.Execute(this, @event);
            Assert.True(@event.Published);
        }

    }
}
