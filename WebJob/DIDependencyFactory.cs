using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NewOrbit.Messaging.Shared;

namespace WebJob
{
    class DIDependencyFactory : IDependencyFactory
    {
        private readonly Lazy<IServiceProvider> serviceProvider;

        public DIDependencyFactory(Lazy<IServiceProvider> serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object Make(Type type)
        {
            // okay for this type, we will need to know what types need to go into its constructor
            // so a bit of reflection madness needs to occur
            var constructors = type.GetConstructors();
            if (constructors.Any())
            {
                var first = constructors[0]; // we only want the 1
                // we need to know the types for each argument
                var parameters = first.GetParameters()
                    .Select(pi => this.serviceProvider.Value.GetService(pi.ParameterType))
                    .ToArray();
                return Activator.CreateInstance(type, parameters);
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }
    }
}