using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    public class CommandDispatcher
    {
        private readonly ICommand command;
        private readonly Type handlingType;
        private readonly IDependencyFactory dependencyFactory;

        public CommandDispatcher(ICommand command, Type handlingType, IDependencyFactory dependencyFactory)
        {
            this.command = command;
            this.handlingType = handlingType;
            this.dependencyFactory = dependencyFactory;
        }

        public async Task Dispatch()
        {
            if (this.handlingType.IsASaga())
            {

            }
            else
            {
                new OneOffCommandDispatcher(this.command, this.handlingType,
                    this.dependencyFactory).Dispatch();
            }
        }
    }
}
