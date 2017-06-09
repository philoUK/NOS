using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NewOrbit.Messaging.Saga
{
    internal static class TypeExtensions
    {
        public static bool IsSaga(this Type type)
        {
            return type.GetTypeInfo()
                .GetInterfaces()
                .Any(i => i == typeof(ISaga));
        }
    }
}
