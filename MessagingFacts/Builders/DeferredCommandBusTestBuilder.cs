using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingFacts.Messages;
using Moq;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging;

namespace MessagingFacts.Builders
{
    class DeferredCommandBusTestBuilder
    {
        private readonly Dictionary<Type, Type> handlers = new Dictionary<Type, Type>();
        private readonly Mock<IDeferredCommandMechanism> mechanism = new Mock<IDeferredCommandMechanism>();
        private readonly Mock<IEventBus> eventBus = new Mock<IEventBus>();
        private ICommand command;

        public void CheckCommandWasNotSubmitted()
        {
            this.mechanism.Verify(m => m.Defer(It.Is<ICommand>(cmd => cmd == this.command)),
                Times.Never());
        }

        public void CheckCommandWasSubmitted()
        {
            this.mechanism.Verify(m => m.Defer(It.Is<ICommand>(cmd => cmd == this.command)),
                Times.AtLeastOnce);
        }

        public void CheckCommandQueuedEventRaised()
        {
            this.eventBus.Verify(m => m.Publish(
                    It.IsAny<object>(),
                    It.Is<IEvent>(@evt => ((CommandWasQueuedEvent) evt).CommandName ==
                                          this.command.GetType().AssemblyQualifiedName)),
                Times.Once());
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

        public async Task<DeferredCommandBusTestBuilder> Submit()
        {
            var bus = new DeferredClientCommandBus(mechanism.Object, eventBus.Object);
            try
            {
                await bus.Submit(this.command).ConfigureAwait(false);
            }
            catch (NoCommandHandlerDefinedException)
            {
            }
            return this;
        }

        public DeferredCommandBusTestBuilder WithHandlerForCommand<T>(Type registeredType)
        {
            var key = typeof(T);
            if (this.handlers.ContainsKey(key))
            {
                this.handlers[key] = registeredType;
            }
            else
            {
                this.handlers.Add(key, registeredType);
            }
            return this;
        }
    }

}
