namespace NewOrbit.Messaging.Abstractions
{
    public interface IAmStartedByCommandOf<in T> : IInvokeCommands where T: ICommand
    {
        void StartByCommand(T command);
    }
}
