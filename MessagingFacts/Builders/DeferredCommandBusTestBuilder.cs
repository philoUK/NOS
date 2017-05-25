using System;
using System.Collections.Generic;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using Xunit;

namespace MessagingFacts.Builders
{
    class DeferredCommandBusTestBuilder
    {
        private readonly Dictionary<Type, Type> handlers = new Dictionary<Type, Type>();
        private readonly Mock<IDeferredCommandMechanism> mechanism = new Mock<IDeferredCommandMechanism>();

        private ICommand command;

        public void CheckCommandWasNotSubmitted()
        {
            this.mechanism.Verify(m => m.Defer(It.Is<ICommand>(cmd => cmd == this.command)),
                Times.Never());
        }

        public DeferredCommandBusTestBuilder WithNoHandlersForCommand<T>() where T: ICommand
        {
            var key = typeof(T);
            if (this.handlers.ContainsKey(key))
            {
                this.handlers.Remove(key);
            }
            return this;
        }

        public DeferredCommandBusTestBuilder WithCommand(TestCommand testCommand)
        {
            this.command = testCommand;
            return this;
        }

        public DeferredCommandBusTestBuilder Submit()
        {
            var registry = new Mock<ICommandHandlerRegistry>();
            registry.Setup(r => r.GetHandlerFor(It.IsAny<ICommand>()))
                .Returns(this.GetHandler());
            this.Defer(registry);
            return this;
        }

        private Type GetHandler()
        {
            var key = this.command?.GetType() ?? typeof(object);
            if (this.handlers.ContainsKey(key))
            {
                return this.handlers[key];
            }
            return null;
        }

        private void Defer(Mock<ICommandHandlerRegistry> registry)
        {
            var bus = new DeferredCommandBus(mechanism.Object, registry.Object);
            try
            {
                bus.Submit(this.command);
            }
            catch (NoCommandHandlerDefinedException)
            {
            }
        }
    }

}
