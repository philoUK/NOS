using System;
using System.Reflection;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Dispatch
{
    internal static class TypeExtensions
    {
        public static bool IsASaga(this Type type)
        {
            return type.IsSubClassOfGenericType(typeof(Saga<>));
        }

        public static void HandleEvent(this object o, IEvent @event)
        {
            try
            {
                var i = o.GetGenericInterface(typeof(ISubscribeToEventsOf<>),
                    @event.GetType());
                var method = i.GetMethod("HandleEvent");
                method.Invoke(o, new object[] {@event});
            }
            catch (TargetInvocationException tex)
            {
                throw tex.InnerException;
            }
        }

        public static void HandleCommand(this object o, ICommand cmd)
        {
            try
            {
                var i = o.GetGenericInterface(typeof(IHandleCommandsOf<>),
                    cmd.GetType());
                var method = i.GetMethod("HandleCommand");
                method.Invoke(o, new object[] {cmd});
            }
            catch (TargetInvocationException tex)
            {
                throw tex.InnerException;
            }
        }
    }
}
