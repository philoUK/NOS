using System;
using System.Collections.Generic;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Abstractions;
using NewOrbit.Messaging.Exceptions;
using Xunit;

namespace MessagingFacts.Utilities
{
    internal class CommandBusTester
    {
        private readonly Dictionary<Type, List<Type>> handlers = new Dictionary<Type, List<Type>>();
        private readonly Mock<IGetCommandHandler> mockGetCommandHandler = new Mock<IGetCommandHandler>();
        private readonly Mock<ICommandBusLogger> mockLogger = new Mock<ICommandBusLogger>();
        private bool multipleHandlerExceptionThrown = false;
        private bool noHandlerExceptionThrown = false;
        public CommandBusTester GivenMessageHandler<T, T1>()
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers.Add(typeof(T), new List<Type>());
            }
            handlers[typeof(T)].Add(typeof(T1));
            return this;
        }


        private IEnumerable<IInvokeCommands> GetHandlers(ICommand cmd)
        {
            var key = cmd.GetType();
            if (this.handlers.ContainsKey(key))
            {
                foreach (var handlingType in this.handlers[key])
                {
                    yield return (IInvokeCommands)Activator.CreateInstance(handlingType);
                }
            }
        }

        public void Execute(ICommand cmd)
        {
            this.mockGetCommandHandler.Setup(h => h.GetCommands(It.IsAny<ICommand>()))
                .Returns(this.GetHandlers(cmd));
            try
            {
                var bus = new CommandBus(this.mockGetCommandHandler.Object, this.mockLogger.Object);
                bus.Submit(cmd);
            }
            catch (MultipleCommandHandlersDefinedException)
            {
                this.multipleHandlerExceptionThrown = true;
            }
            catch (NoCommandHandlersDefinedException)
            {
                this.noHandlerExceptionThrown = true;
            }
        }

        public void CheckMultipleCommandDefinedExceptionWasThrownAndLogged()
        {
            Assert.True(this.multipleHandlerExceptionThrown);
            this.mockLogger.Verify(l => l.LogMultipleCommandHandlerException(It.IsAny<ICommand>()),
                Times.AtLeastOnce());
        }

        public void CheckNoHandlerDefinedExceptionWasThrownAndLogged()
        {
            Assert.True(this.noHandlerExceptionThrown);
            this.mockLogger.Verify(l => l.LogNoCommandHandlerException(It.IsAny<ICommand>()),
                Times.AtLeastOnce());
        }
    }
}
