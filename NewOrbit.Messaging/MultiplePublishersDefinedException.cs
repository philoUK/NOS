using System;

namespace NewOrbit.Messaging
{
    public class MultiplePublishersDefinedException : Exception
    {
        public MultiplePublishersDefinedException(IEvent @event)
            :base($"There can only be 1 publisher for event {@event.GetType().Name}")
        {
        }

    }
}