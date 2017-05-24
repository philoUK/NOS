using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Utilities
{
    internal class FakeCommand : ICommand
    {
        public Guid UniqueIdentifier => Guid.NewGuid();
        public bool WasExecuted { get; set; }
    }

    internal class FakeCommandHandlerOne : IHandleCommandOf<FakeCommand>
    {
        public void HandleCommand(FakeCommand message)
        {
            message.WasExecuted = true;
        }

        public void InvokeCommand(ICommand cmd)
        {
            HandleCommand((FakeCommand)cmd);
        }
    }

    internal class FakeCommandHandlerTwo : IAmStartedByCommandOf<FakeCommand>
    {
        public void StartByCommand(FakeCommand command)
        {
        }

        public void InvokeCommand(ICommand cmd)
        {
        }
    }
}
