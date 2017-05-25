using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    /// <summary>
    /// For testing, do not implement IHandleCommandsOf<OrphanedTestCommand> or 
    /// unit tests will fail
    /// </summary>
    public class OrphanedTestCommand : ICommand
    {
    }
}
