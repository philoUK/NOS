using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class ReceivingCommandBusFacts
    {
        // error unpacking message -- no dispatch, and error retrieving message event logged and rethrow

        // error with handler (0 handlers, more than 1) -- no dispatch and error with handler event logged, and rethrow

        // error dispatching, raise error event and rethrow

        // dispatch ok, raise event and no exception

        [Fact]
        public async Task ErrorUnpackingMessage()
        {
            var builder = new ReceivingCommandBusTestBuilder()
                .GivenAnErrorUnpackingAMessage();
            await builder.Receive();
            builder.VerifyMessageUnpackingExceptionIsThrown();
            builder.VerifyUnpackingErrorEventIsRaised();
        }
    }
}
