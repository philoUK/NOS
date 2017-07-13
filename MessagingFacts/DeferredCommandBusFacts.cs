using System;
using System.Threading.Tasks;
using MessagingFacts.Builders;
using MessagingFacts.Messages;
using Xunit;

namespace MessagingFacts
{
    public class DeferredCommandBusFacts
    {
        private Task<DeferredCommandBusTestBuilder> CreateBuilder()
        {
            return new DeferredCommandBusTestBuilder()
                .WithHandlerForCommand<TestCommand>(this.GetType())
                .WithCommand(new TestCommand())
                .Submit();
        }

        [Fact]
        public async Task SingleRegisteredHandlerQueuesTheCommand()
        {
            var m = await CreateBuilder();
            m.CheckCommandWasSubmitted();
        }

        [Fact]
        public async Task SingleRegisteredHandlerRaisesCommandQueuedEvent()
        {
            var m = await CreateBuilder();
            m.CheckCommandQueuedEventRaised();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\n")]
        public async Task CommandsMustHaveAValidId(string id)
        {
            var m = new DeferredCommandBusTestBuilder()
                .WithHandlerForCommand<TestCommand>(this.GetType())
                .WithCommand(new TestCommand {Id = id});
            await Assert.ThrowsAsync<ArgumentException>(() => m.Submit());
        }

    }
}
