using MessagingFacts.Utilities;
using NewOrbit.Messaging;
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

        //[Fact]
        //public void DealsWithNoSubscribersToAGivenEvent()
        //{
        //    var builder = new PublisherBusTester()
        //        .GivenNoSubscribersForEvent<FakeEvent>();
        //    var @event = new FakeEvent();
        //    builder.Execute(this,new FakeEvent());
        //    Assert.False(@event.HandledBySubscriberOne);
        //    Assert.False(@event.HandledBySubscriberTwo);
        //}

        //[Fact]
        //public void PassesEventToEachSubscriber()
        //{
        //    var builder = new PublisherBusTester()
        //        .GivenPublisher<FakeEvent, PublisherBusFacts>()
        //        .GivenSubscriber<FakeEvent, FakeEventSubscriberOne>()
        //        .GivenSubscriber<FakeEvent, FakeEventSubscriberTwo>();
        //    var @event = new FakeEvent();
        //    builder.Execute(this,@event);
        //    Assert.True(@event.HandledBySubscriberOne);
        //    Assert.True(@event.HandledBySubscriberTwo);
        //}

        //[Fact]
        //public void OnlyAllowsASinglePublisher()
        //{

        //}
    }
}
