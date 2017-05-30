using System.Threading.Tasks;
using NewOrbit.Messaging.Monitoring.Events;

namespace NewOrbit.Messaging.Command
{
    public class DeferredCommandBus : ICommandBus
    {
        private readonly IDeferredCommandMechanism mechanism;
        private readonly ICommandHandlerRegistry registry;
        private readonly IEventBus eventBus;

        public DeferredCommandBus(IDeferredCommandMechanism mechanism, ICommandHandlerRegistry registry, IEventBus eventBus)
        {
            this.mechanism = mechanism;
            this.registry = registry;
            this.eventBus = eventBus;
        }

        public async Task Submit(ICommand command)
        {
            EnsureThereIsASingleHandlerAvailable(command);
            await this.SubmitCommand(command).ConfigureAwait(false);
            await this.NotifyCommandWasSubmitted(command).ConfigureAwait(false);
        }

        private void EnsureThereIsASingleHandlerAvailable(ICommand command)
        {
            var hasHandler = this.registry.GetHandlerFor(command);
            if (hasHandler == null)
            {
                throw new NoCommandHandlerDefinedException(command);
            }
        }

        private async Task SubmitCommand(ICommand command)
        {
            await this.mechanism.Defer(command).ConfigureAwait(false);
        }

        private async Task NotifyCommandWasSubmitted(ICommand command)
        {
            var evt = new CommandWasQueuedEvent {CommandName = command.GetType().AssemblyQualifiedName};
            await this.eventBus.Publish(evt).ConfigureAwait(false);
        }
    }
}