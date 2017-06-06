namespace NewOrbit.Messaging
{
    public interface IHandleCommandsOf<T> where T: ICommand
    {
        void HandleCommand(T command);
    }
}