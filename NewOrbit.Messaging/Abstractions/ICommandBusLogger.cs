namespace NewOrbit.Messaging.Abstractions
{
    public interface ICommandBusLogger
    {
        void LogMultipleCommandHandlerException(ICommand command);
        void LogNoCommandHandlerException(ICommand cmd);
    }
}