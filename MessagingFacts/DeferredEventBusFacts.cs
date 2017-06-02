using System.Threading.Tasks;
using MessagingFacts.Builders;
using MessagingFacts.Messages;
using Xunit;

namespace MessagingFacts
{
    public class DeferredEventBusFacts
    {
        [Fact]
        public async Task NoPublisherThrowsAnException()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithNoPublishersForEvent<CommandTestedEvent>()
                .Submit();
            m.CheckNoPublisherExceptionThrown();
        }

        [Fact]
        public async Task NoSubscribersLogsTheFactButDoesNotFail()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithPublisherForEvent()
                .Submit();
            m.CheckNoSubscribersWasLogged();
        }

        [Fact]
        public async Task EachSubscriberShouldHaveTheirEventQueuedUp()
        {
            var subscriberCount = 100;
            var m = await new DeferredEventBusTestBuilder()
                .WithPublisherForEvent()
                .WithMultipleSubscribersToEvent(subscriberCount)
                .Submit();
            m.CheckEachMessageWasQueuedUp();
        }

        [Fact]
        public async Task TheWrongPublisherShouldThrowAnException()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithIncorrectPublisherForEvent()
                .Submit();
            m.CheckWrongPublisherExceptionThrown();
        }
    }
}
