namespace NewOrbit.Messaging.Registrars
{
    public interface IHandleCommandsOf<T> where T: ICommand
    {
        void Handle(T command);
    }
}