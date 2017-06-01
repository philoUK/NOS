using System;

namespace NewOrbit.Messaging.Event
{
    public class NoEventPublisherFoundException : Exception
    {
        public NoEventPublisherFoundException(IEvent @event)
            :base($"No registered publishers for event {@event?.GetType().AssemblyQualifiedName ?? "N/A"}")
        {
        }
    }
}