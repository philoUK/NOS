using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class ReceivingEventBusFacts
    {
        // queue message -- if the event within is bollocks, then
        // log an exception and throw it

        // if the subscribing type is bollocks, then log an exception and throw it

        // send it to the subscribing type (with the DI shim interface), and publish 
        // the Event handled event if it worked

        [Fact]
        public async Task GarbledEventThrowsCorrectException()
        {
            var builder = await new ReceivingEventBusTestBuilder()
                .GivenAnEventWithBadInput()
                .SubmitEvent();
            builder.CheckEventUnpackingExceptionWasThrown();
        }

        [Fact]
        public async Task GarbledSubscriberTypeThrowsCorrectException()
        {
            var builder = await new ReceivingEventBusTestBuilder()
                .GivenABadSubscriberType()
                .SubmitEvent();
            builder.CheckEventUnpackingExceptionWasThrown();
        }

        [Fact]
        public async Task EventGetsProperlyDispatchedAndEventDispatchEventRaised()
        {
            var builder = await new ReceivingEventBusTestBuilder()
                .SubmitEvent();
            builder.CheckEventWasDispatched();
            builder.CheckEventDispatchEventRaised();
        }
    }
}
