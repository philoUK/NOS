using MessagingFacts.Utilities;
using Xunit;

namespace MessagingFacts
{
    public class PublisherBusFacts
    {
        [Fact]
        public void DealsWithNoSubscribersToAGivenEvent()
        {
            var builder = new PublisherBusTester()
                .GivenNoSubscribersForEvent<FakeEvent>();
            var @event = new FakeEvent();
            builder.Execute(new FakeEvent());
            Assert.False(@event.HandledBySubscriberOne);
            Assert.False(@event.HandledBySubscriberTwo);
        }

        [Fact]
        public void PassesEventToEachSubscriber()
        {
            var builder = new PublisherBusTester()
                .GivenSubscriber<FakeEvent, FakeEventSubscriberOne>()
                .GivenSubscriber<FakeEvent, FakeEventSubscriberTwo>();
            var @event = new FakeEvent();
            builder.Execute(@event);
            Assert.True(@event.HandledBySubscriberOne);
            Assert.True(@event.HandledBySubscriberTwo);
        }

        [Fact]
        public void OnlyAllowsASinglePublisher()
        {
            Assert.False(true);
        }
    }
}
