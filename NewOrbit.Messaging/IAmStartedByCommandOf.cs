namespace NewOrbit.Messaging
{
    public interface IAmStartedByCommandOf<in T> : IInvokeCommands where T: ICommand
    {
        void StartByCommand(T command);
    }
}
