using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using NewOrbit.Messaging.Command;

namespace NewOrbit.Messaging.Registrars
{
    public class CommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly Dictionary<Type,List<Type>> cachedHandlers = new Dictionary<Type, List<Type>>();

        public CommandHandlerRegistry()
        {
            foreach (var assembly in GetReferencingAssemblies())
            {
                foreach (var exportedType in assembly.GetExportedTypes())
                {
                    foreach (var interfaceType in exportedType.GetInterfaces()
                        .Select(i => i.GetTypeInfo())
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleCommandsOf<>)))
                    {
                        var arg = interfaceType.GetGenericArguments()[0];
                        if (!this.cachedHandlers.ContainsKey(arg))
                        {
                            this.cachedHandlers.Add(arg, new List<Type>());
                        }
                        this.cachedHandlers[arg].Add(exportedType);
                    }
                }
            }
        }

        public Type GetHandlerFor(ICommand command)
        {
            var key = command.GetType();
            if (this.cachedHandlers.ContainsKey(key))
            {
                return this.cachedHandlers[key].FirstOrDefault();
            }
            return null;
        }

        private static IEnumerable<Assembly> GetReferencingAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, "NewOrbit.Messaging.Registrars"))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }
    }
}