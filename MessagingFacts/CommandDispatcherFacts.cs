using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class CommandDispatcherFacts
    {
        [Fact]
        public async Task OneOffHandlerIsHandledViaDependencyFactory()
        {
            var builder = await new CommandDispatcherBuilder()
                .GivenOneOffCommandAndHandler()
                .Execute();
            builder.AssertSagaPathWasNotTaken();
            builder.AssertCommandHandled();
        }

        [Fact]
        public async Task NewSagaHandlerIsHandledCorrectly()
        {
            var builder = await new CommandDispatcherBuilder()
                .GivenOneOffCommandAndNewSagaHandler()
                .Execute();
            builder.AssertNewSagaPathWasTaken();
            builder.AssertCommandHandled();
        }

        [Fact]
        public async Task ExistingSagaHandlerIsHandledCorrectly()
        {
            var builder = await new CommandDispatcherBuilder()
                .GivenOneOffCommandAndExistingSagaHandler()
                .Execute();
            builder.AssertExistingSagaPathWasTaken();
            builder.AssertCommandHandled();
        }
    }
}
