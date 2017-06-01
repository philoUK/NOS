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
                .WithEvent()
                .Submit();
            m.CheckNoPublisherExceptionThrown();
        }

        [Fact]
        public async Task NoSubscribersLogsTheFactButDoesNotFail()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithPublisherForEvent()
                .WithEvent()
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
            
        }
    }
}
