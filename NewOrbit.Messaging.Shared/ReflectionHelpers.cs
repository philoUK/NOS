using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace NewOrbit.Messaging.Shared
{
    public static class ReflectionHelpers
    {
        public static IEnumerable<Tuple<Type, Type>> TypesThatImplementInterface(Func<Type,bool> comparer, string basedOff)
        {
            foreach (var assembly in GetReferencingAssemblies(basedOff))
            {
                foreach (var exportedType in assembly.GetExportedTypes())
                {
                    foreach (var interfaceType in exportedType.GetInterfaces()
                        .Select(i => i.GetTypeInfo())
                        .Where(i => i.IsGenericType && comparer(i.GetGenericTypeDefinition())))
                    {
                        var arg = interfaceType.GetGenericArguments()[0];
                        yield return new Tuple<Type, Type>(arg, exportedType);
                    }
                }
            }
        }

        private static IEnumerable<Assembly> GetReferencingAssemblies(string basedOff)
        {
            var assemblies = new List<Assembly>();
            var entryAssembly = Assembly.GetEntryAssembly();
            var dependencies = DependencyContext.Load(entryAssembly).RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, basedOff))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Dependencies.Any(d => d.Name == assemblyName);
        }
    }
}
