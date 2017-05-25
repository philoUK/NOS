using System;
using NewOrbit.Messaging.Abstractions;

namespace NewOrbit.Messaging.Exceptions
{
    public class MultipleCommandHandlersDefinedException : InvalidOperationException
    {
        public MultipleCommandHandlersDefinedException(ICommand cmd)
            :base($"More than one command handler was defined for command {cmd.GetType().Name}")
        {
            
        }
    }
}
