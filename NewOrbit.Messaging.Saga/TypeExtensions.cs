using System;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga
{
    internal static class TypeExtensions
    {
        public static bool IsSaga(this Type type)
        {
            return type.IsSubClassOfGenericType(typeof(Saga<>));
        }
    }
}
