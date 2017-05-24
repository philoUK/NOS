namespace NewOrbit.Messaging
{
    public interface IInvokeCommands
    {
        void InvokeCommand(ICommand cmd);
    }
}