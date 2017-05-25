using System;

namespace NewOrbit.Messaging.Command
{
    public interface ICommandHandlerRegistry
    {
        Type GetHandlerFor(ICommand command);
    }
}