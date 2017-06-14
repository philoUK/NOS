using System;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    internal class OneOffCommandDispatcher
    {
        private readonly ICommand command;
        private readonly Type handlingType;
        private readonly IDependencyFactory dependencyFactory;

        public OneOffCommandDispatcher(ICommand command, Type handlingType, IDependencyFactory dependencyFactory)
        {
            this.command = command;
            this.handlingType = handlingType;
            this.dependencyFactory = dependencyFactory;
        }

        public void Dispatch()
        {
            var target = this.dependencyFactory.Make(this.handlingType);
            target.HandleCommand(this.command);
        }
    }
}