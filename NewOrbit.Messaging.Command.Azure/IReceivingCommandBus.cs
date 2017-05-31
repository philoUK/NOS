namespace NewOrbit.Messaging.Command
{
    public interface IReceivingCommandBus
    {
        void Handle(object command);
    }
}