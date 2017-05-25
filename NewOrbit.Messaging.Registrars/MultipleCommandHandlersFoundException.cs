using System;

namespace NewOrbit.Messaging.Registrars
{
    public class MultipleCommandHandlersFoundException : Exception
    {
        public MultipleCommandHandlersFoundException(ICommand command)
            :base($"More than 1 command handler was found for type {command.GetType().Name}")
        {
            
        }
    }
}