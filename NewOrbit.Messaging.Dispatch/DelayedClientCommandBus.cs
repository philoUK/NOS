using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Dispatch
{
    internal class DelayedClientCommandBus : IClientCommandBus
    {
        private readonly List<ICommand> delayedCommands = new List<ICommand>();

        public Task Submit(ICommand command)
        {
            this.delayedCommands.Add(command);
            return Task.CompletedTask;
        }

        public IEnumerable<ICommand> DelayedCommands => this.delayedCommands;
    }
}
