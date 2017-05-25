namespace NewOrbit.Messaging.Command
{
    public interface IDeferredCommandMechanism
    {
        void Defer(ICommand command);
    }
}