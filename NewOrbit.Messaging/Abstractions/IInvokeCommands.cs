namespace NewOrbit.Messaging.Abstractions
{
    public interface IInvokeCommands
    {
        void InvokeCommand(ICommand cmd);
    }
}