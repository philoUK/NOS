using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Registrars;

namespace NewOrbit.Messaging.Command
{
    public class DeferredClientCommandBus : IClientCommandBus, IPublishEventsOf<CommandWasQueuedEvent>
    {
        private readonly IDeferredCommandMechanism mechanism;
        private readonly IEventBus eventBus;

        public DeferredClientCommandBus(IDeferredCommandMechanism mechanism, IEventBus eventBus)
        {
            this.mechanism = mechanism;
            this.eventBus = eventBus;
        }

        public async Task Submit(ICommand command)
        {
            this.EnsureCommandHasAValidId(command);
            await this.SubmitCommand(command).ConfigureAwait(false);
            await this.NotifyCommandWasSubmitted(command).ConfigureAwait(false);
        }

        private void EnsureCommandHasAValidId(ICommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
            {
                throw new ArgumentException("Command Id must be set");
            }
        }

        private async Task SubmitCommand(ICommand command)
        {
            await this.mechanism.Defer(command).ConfigureAwait(false);
        }

        private async Task NotifyCommandWasSubmitted(ICommand command)
        {
            var evt = new CommandWasQueuedEvent {CommandName = command.GetType().AssemblyQualifiedName};
            await this.eventBus.Publish(this,evt).ConfigureAwait(false);
        }
    }
}