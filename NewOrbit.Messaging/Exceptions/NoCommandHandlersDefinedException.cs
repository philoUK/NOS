using System;
using NewOrbit.Messaging.Abstractions;

namespace NewOrbit.Messaging.Exceptions
{
    public class NoCommandHandlersDefinedException : InvalidOperationException
    {

        public NoCommandHandlersDefinedException(ICommand cmd)
            :base($"No command handler found for command of {cmd.GetType().Name}")
        {
        }
    }
}