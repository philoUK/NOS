namespace NewOrbit.Messaging
{
    public interface ICommandBusLogger
    {
        void LogMultipleCommandHandlerException(ICommand command);
        void LogNoCommandHandlerException(ICommand cmd);
    }
}