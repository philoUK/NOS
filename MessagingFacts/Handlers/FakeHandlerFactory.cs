using System;
using NewOrbit.Messaging.Shared;

namespace MessagingFacts.Handlers
{
    internal class FakeHandlerFactory : IHandlerFactory
    {
        public object Make(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
