using System;

namespace NewOrbit.Messaging.Shared
{
    public interface IHandlerFactory
    {
        object Make(Type type);
    }
}