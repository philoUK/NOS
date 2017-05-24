using System;

namespace NewOrbit.Messaging
{
    public interface ICommand
    {
        Guid UniqueIdentifier { get; }
    }
}
