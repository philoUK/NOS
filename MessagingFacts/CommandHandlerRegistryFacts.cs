using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Registrars;
using TestExtras;
using Xunit;

namespace MessagingFacts
{
    public class CommandHandlerRegistryFacts
    {
        [Fact]
        public void DoesNotFindAnyTypesThatImplementOrphanedTestCommand()
        {
            var sut = new CommandHandlerRegistry();
            Assert.Null(sut.GetHandlerFor(new OrphanedTestCommand()));
        }

        [Fact]
        public void FindsATypeInSameAssemblyThatImplmentsTestCommand()
        {
            var sut = new CommandHandlerRegistry();
            Assert.Equal(typeof(TestCommandHandler), sut.GetHandlerFor(new TestCommand()));
        }

        [Fact]
        public void FindsATypeInDifferentAssemblyThatImplementsACommand()
        {
            var sut = new CommandHandlerRegistry();
            Assert.Equal(typeof(ExternalCommandHandler), sut.GetHandlerFor(new ExternalCommand()));
        }

        [Fact]
        public void MultipleHandlersNotAllowed()
        {
            var sut = new CommandHandlerRegistry();
            Assert.Throws<MultipleCommandHandlersFoundException>(() => sut.GetHandlerFor(new BadCommand()));
        }
    }
}
