using System;

namespace NewOrbit.Messaging
{
    public interface IEvent
    {
        Guid UniqueIdentifier { get; }
    }
}
