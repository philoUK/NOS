using System;
using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Messages;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Saga.Commands;
using Xunit;

namespace MessagingFacts
{
    public class SagaRegisterTimeoutFacts
    {
        private readonly Mock<IClientCommandBus> cmdBus = new Mock<IClientCommandBus>();
        private readonly Mock<IEventBus> evtBus = new Mock<IEventBus>();
        private readonly Mock<ISagaDatabase> sagaDb = new Mock<ISagaDatabase>();

        // when a saga wishes to register a timeout, it simply calls RegisterTimeout(nameof(this.SomeFunction), Time.Minutes(15))
        [Fact]
        public async Task RegisterTimeoutCreatesTheAppropriateMessage()
        {
            var cmd = new StartTimeoutCommand();
            var dispatcher = new CommandDispatcher(cmd, typeof(TimingSaga), new FakeHandlerFactory(),
                this.sagaDb.Object, this.cmdBus.Object, this.evtBus.Object);
            await dispatcher.Dispatch();
            this.VerifyTimeoutMessage(cmd.CorrelationId);
        }

        private void VerifyTimeoutMessage(string correlationId)
        {
            this.cmdBus.Verify(bus => bus.Submit(It.Is<ICommand>(cmd => this.CheckTimeoutMessage(correlationId, cmd))),
                Times.Once());
        }

        private bool CheckTimeoutMessage(string id, ICommand cmd)
        {
            var timeoutCmd = cmd as RegisterTimeoutCommand;
            return timeoutCmd != null &&
                    timeoutCmd.CorrelationId == id &&
                    timeoutCmd.MethodName == nameof(TimingSaga.OnTimeout) &&
                    timeoutCmd.Timeout > DateTime.UtcNow;
        }

        [Fact]
        public async Task FailsWhenTheTimeoutRegistrationCallbackDoesNotExist()
        {
            var cmd = new StartTimeoutCommand();
            var dispatcher = new CommandDispatcher(cmd, typeof(InvalidTimingSaga), new FakeHandlerFactory(), 
                this.sagaDb.Object, this.cmdBus.Object, this.evtBus.Object);
            await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.Dispatch());
        }
    }
}
