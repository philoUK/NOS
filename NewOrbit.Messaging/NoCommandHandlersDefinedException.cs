using System;

namespace NewOrbit.Messaging
{
    public class NoCommandHandlersDefinedException : InvalidOperationException
    {

        public NoCommandHandlersDefinedException(ICommand cmd)
            :base($"No command handler found for command of {cmd.GetType().Name}")
        {
        }
    }
}