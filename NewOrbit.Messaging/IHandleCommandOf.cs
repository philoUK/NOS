namespace NewOrbit.Messaging
{
    public interface IHandleCommandOf<in T> :IInvokeCommands where T: ICommand
    {
        void HandleCommand(T message);
    }
}
