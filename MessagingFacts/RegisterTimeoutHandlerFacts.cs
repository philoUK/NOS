using System;
using Moq;
using NewOrbit.Messaging.Saga.Commands;
using NewOrbit.Messaging.Timeouts;
using Xunit;

namespace MessagingFacts
{
    public class RegisterTimeoutHandlerFacts
    {
        private readonly Mock<ITimeoutDatabase> database = new Mock<ITimeoutDatabase>();

        [Fact]
        public void RequestIsStoredInTheTimeoutDatabase()
        {
            var sut = new RegisterTimeoutHandler(this.database.Object);
            var msg = new RegisterTimeoutCommand
            {
                CorrelationId = "correlationId",
                Id = "id",
                MethodName = "MethodName",
                Timeout = DateTime.UtcNow
            };
            sut.HandleCommand(msg);
            this.database.Verify(db => db.Save(It.Is<TimeoutData>(data => this.CheckData(msg,data))), Times.Once());
        }

        private bool CheckData(RegisterTimeoutCommand cmd, TimeoutData timeoutData)
        {
            return timeoutData.TargetId == cmd.CorrelationId && timeoutData.TargetMethod == cmd.MethodName &&
                   timeoutData.Timeout == cmd.Timeout;
        }
    }
}
