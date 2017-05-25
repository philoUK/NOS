using System;

namespace NewOrbit.Messaging.Abstractions
{
    public interface ICommand
    {
        Guid UniqueIdentifier { get; }
    }
}
