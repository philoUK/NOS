using System;

namespace NewOrbit.Messaging.Exceptions
{
    public class UnregisteredPublisherException : Exception
    {
        public UnregisteredPublisherException(object publisher, IEvent @event)
            :base($"{publisher.GetType().Name} is not the publisher for events of type {@event.GetType().Name}")
        {
        }
    }
}