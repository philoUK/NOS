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
            return library.Dependencies.Any(
                d => d.Name.ToLower().StartsWith(assemblyName.ToLower()));
        }

        public static TypeInfo GetGenericInterface(this object lhs, Func<Type,bool> fComparer, Type internalType)
        {
            return lhs.GetType()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .FirstOrDefault(i => i.IsGenericType && fComparer(i.GetGenericTypeDefinition()) &&
                                     i.GetGenericArguments()[0] == internalType);
        }

        public static bool IsSubClassOfGenericType(this Type typeToCheck, Type genericType)
        {
            while (typeToCheck != null && typeToCheck != typeof(object))
            {
                var cur = typeToCheck.GetTypeInfo().IsGenericType
                    ? typeToCheck.GetTypeInfo().GetGenericTypeDefinition()
                    : typeToCheck;
                if (genericType == cur)
                {
                    return true;
                }
                typeToCheck = typeToCheck.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}
