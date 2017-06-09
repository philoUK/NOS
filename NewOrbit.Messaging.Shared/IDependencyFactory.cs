using System;

namespace NewOrbit.Messaging.Shared
{
    public interface IDependencyFactory
    {
        object Make(Type type);
    }
}