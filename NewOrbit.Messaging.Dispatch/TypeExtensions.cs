using System;
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
            var i = o.GetGenericInterface(typeof(ISubscribeToEventsOf<>),
                @event.GetType());
            var method = i.GetMethod("HandleEvent");
            method.Invoke(o, new object[] { @event });
        }

        public static void HandleCommand(this object o, ICommand cmd)
        {
            var i = o.GetGenericInterface(typeof(IHandleCommandsOf<>),
                cmd.GetType());
            var method = i.GetMethod("HandleCommand");
            method.Invoke(o, new object[] {cmd});
        }
    }
}
