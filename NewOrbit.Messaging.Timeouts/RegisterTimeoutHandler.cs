using NewOrbit.Messaging.Saga.Commands;

namespace NewOrbit.Messaging.Timeouts
{
    public class RegisterTimeoutHandler : IHandleCommandsOf<RegisterTimeoutCommand>
    {
        private readonly ITimeoutDatabase timeoutDatabase;

        public RegisterTimeoutHandler(ITimeoutDatabase timeoutDatabase)
        {
            this.timeoutDatabase = timeoutDatabase;
        }

        public void HandleCommand(RegisterTimeoutCommand command)
        {
            var data = new TimeoutData
            {
                TargetId = command.CorrelationId,
                TargetMethod = command.MethodName,
                Timeout = command.Timeout,
                TargetType = command.OwnerType
            };
            this.timeoutDatabase.Save(data);
        }
    }
}