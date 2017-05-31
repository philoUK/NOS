using System;

namespace NewOrbit.Messaging.Command
{
    public class NoCommandHandlerDefinedException : Exception
    {
        public NoCommandHandlerDefinedException(ICommand command)
            :base($"No command handler found for command of type {command?.GetType().Name ?? "N/A"}")
        { 
        }
    }
}