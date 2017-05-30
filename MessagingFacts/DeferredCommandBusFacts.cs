using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MessagingFacts.Builders;
using MessagingFacts.Messages;
using Xunit;

namespace MessagingFacts
{
    public class DeferredCommandBusFacts
    {
        [Fact]
        public async Task NoRegisteredHandlerThrowsAnException()
        {
            var m = await new DeferredCommandBusTestBuilder()
                .WithNoHandlersForCommand<TestCommand>()
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandWasNotSubmitted();
        }

        [Fact]
        public async Task SingleRegisteredHandlerQueuesTheCommand()
        {
            var m = await new DeferredCommandBusTestBuilder()
                .WithHandlerForCommand<TestCommand>(this.GetType())
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandWasSubmitted();
        }

        [Fact]
        public async Task SingleRegisteredHandlerRaisesCommandQueuedEvent()
        {
            var m = await new DeferredCommandBusTestBuilder()
                .WithHandlerForCommand<TestCommand>(this.GetType())
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandQueuedEventRaised();
        }

    }
}
