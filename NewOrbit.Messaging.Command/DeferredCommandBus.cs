namespace NewOrbit.Messaging.Command
{
    public class DeferredCommandBus
    {
        private readonly IDeferredCommandMechanism mechanism;
        private readonly ICommandHandlerRegistry registry;

        public DeferredCommandBus(IDeferredCommandMechanism mechanism, ICommandHandlerRegistry registry)
        {
            this.mechanism = mechanism;
            this.registry = registry;
        }

        public void Submit(ICommand command)
        {
            EnsureThereIsASingleHandlerAvailable(command);
            this.mechanism.Defer(command);
        }

        private void EnsureThereIsASingleHandlerAvailable(ICommand command)
        {
            var hasHandler = this.registry.GetHandlerFor(command);
            if (hasHandler == null)
            {
                throw new NoCommandHandlerDefinedException(command);
            }
        }
    }
}