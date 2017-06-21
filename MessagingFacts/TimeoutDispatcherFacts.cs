using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class TimeoutDispatcherFacts
    {
        [Fact]
        public async Task CancellationTokenWorks()
        {
            var builder = new TimeoutDispatcherTestBuilder()
                .GivenNoTimeouts()
                .GivenADelay()
                .ThenThrowsTheAppropriateException();
            await builder.Execute();
        }

        [Fact]
        public async Task AnErrorWhenCallingTheSagaTimeoutMethodWillNotPerformOtherActions()
        {
            var builder = new TimeoutDispatcherTestBuilder()
                .GivenATimeoutToASagaThatErrors()
                .GivenADelay()
                .ThenNoSagaCommandsAreSent()
                .ThenNoSagaEventsArePublished()
                .ThenTheTimeoutIsNotDeleted();
            await builder.Execute();
        }

        [Fact]
        public async Task EndToEndDispatchPerformsActionsAsExpected()
        {
            var builder = new TimeoutDispatcherTestBuilder()
                .GivenATimeoutToASagaThatSucceeds()
                .GivenADelay()
                .ThenSagaCommandsAreSent()
                .ThenSagaEventsArePublished()
                .ThenTheTimeoutIsDeleted();
            await builder.Execute();
        }
    }
}
