using MessagingFacts.Builders;
using MessagingFacts.Messages;
using Xunit;

namespace MessagingFacts
{
    public class DeferredCommandBusFacts
    {
        // if the command does not have a 
        // handler, throw an exception

        // The command must have an Id

        // if the deferral mechanism isn't working
        // throw an exception

        [Fact]
        public void NoRegisteredHandlerThrowsAnException()
        {
            var m = new DeferredCommandBusTestBuilder()
                .WithNoHandlersForCommand<TestCommand>()
                .WithCommand(new TestCommand())
                .Submit();
            m.CheckCommandWasNotSubmitted();
        }

    }
}
