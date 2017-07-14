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
        public async Task TheWrongPublisherShouldThrowAnException()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithIncorrectPublisherForEvent()
                .Submit();
            m.CheckWrongPublisherExceptionThrown();
        }

        [Fact]
        public async Task SubmitsCorrectlyToDeferralMechanism()
        {
            var m = await new DeferredEventBusTestBuilder()
                .WithPublisherForEvent()
                .Submit();
            m.CheckEventWasProperlyQueued();
        }
    }
}
