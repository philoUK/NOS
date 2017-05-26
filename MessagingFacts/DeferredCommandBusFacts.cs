using MessagingFacts.Builders;
using MessagingFacts.Messages;
using Xunit;

namespace MessagingFacts
{
    public class DeferredCommandBusFacts
    {
        [Fact]
        public void NoRegisteredHandlerThrowsAnException()
        {
            var m = new DeferredCommandBusTestBuilder()
                .WithNoHandlersForCommand<TestCommand>()
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandWasNotSubmitted();
        }

        [Fact]
        public void SingleRegisteredHandlerQueuesTheCommand()
        {
            var m = new DeferredCommandBusTestBuilder()
                .WithHandlerForCommand<TestCommand>(this.GetType())
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandWasSubmitted();
        }

    }
}
