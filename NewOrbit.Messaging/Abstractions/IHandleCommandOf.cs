namespace NewOrbit.Messaging.Abstractions
{
    public interface IHandleCommandOf<in T> :IInvokeCommands where T: ICommand
    {
        void HandleCommand(T message);
    }
}
