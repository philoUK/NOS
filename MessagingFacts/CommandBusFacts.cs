using System;
using System.Collections.Generic;
using MessagingFacts.Utilities;
using Moq;
using NewOrbit.Messaging;
using Xunit;

namespace MessagingFacts
{
    public class CommandBusFacts
    {
        // there can only ever be 1 handler for any given command, be it naked handler or saga
        
        // there can only ever be 1 publisher for any given event, but unlimited subscribers to any given
        // event

        // a saga *should* only ever dispatch to naked handlers in its action methods, it should never do
        // any direct database or service manipulation itself.  Review the code hard and stop this from
        // happening.  This is to ensure the messages can be safely replayed.

        // sagas should bind everything they do through a unique id

        [Fact]
        public void MoreThanOneHandlerThrowsAnException()
        {
            // arrange
            var builder = new CommandBusTester()
                .GivenMessageHandler<FakeCommand, FakeCommandHandlerOne>()
                .GivenMessageHandler<FakeCommand, FakeCommandHandlerTwo>();
            // act
            builder.Execute(new FakeCommand());
            // assert
            builder.CheckMultipleCommandDefinedExceptionWasThrownAndLogged();
        }

        [Fact]
        public void NoHandlersThrowsAnException()
        {
            var builder = new CommandBusTester();
            builder.Execute(new FakeCommand());
            builder.CheckNoHandlerDefinedExceptionWasThrownAndLogged();
        }

        [Fact]
        public void SingleHandlerInvokesTheHandler()
        {
            var builder = new CommandBusTester();
            builder.GivenMessageHandler<FakeCommand, FakeCommandHandlerOne>();
            var msg = new FakeCommand();
            builder.Execute(msg);
            Assert.True(msg.WasExecuted);
        }
    }
}
