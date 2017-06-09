using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace MessagingFacts.Handlers
{
    internal class FakeHandlerFactory : IHandlerFactory, IDependencyFactory
    {
        public Task<object> Make(Type type, IMessage msg)
        {
            return Task.FromResult(Activator.CreateInstance(type));
        }

        public object Make(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
