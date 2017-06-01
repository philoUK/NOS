namespace NewOrbit.Messaging
{
    public interface IHandleCommandsOf<T> where T: ICommand
    {
        void Handle(T command);
    }
}