using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Command
{
    public class CommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly Dictionary<Type,List<Type>> cachedHandlers = new Dictionary<Type, List<Type>>();

        public CommandHandlerRegistry()
        {
            foreach (var tuple in ReflectionHelpers.TypesThatImplementInterface(t => t == typeof(IHandleCommandsOf<>),
                "NewOrbit.Messaging"))
            {
                var eventType = tuple.Item1;
                var handlerType = tuple.Item2;
                if (!this.cachedHandlers.ContainsKey(eventType))
                {
                    this.cachedHandlers.Add(eventType, new List<Type>());
                }
                this.cachedHandlers[eventType].Add(handlerType);
            }
        }

        public Type GetHandlerFor(ICommand command)
        {
            var key = command.GetType();
            if (this.cachedHandlers.ContainsKey(key))
            {
                if (this.cachedHandlers[key].Count > 1)
                {
                    throw new MultipleCommandHandlersFoundException(command);
                }
                return this.cachedHandlers[key].Single();
            }
            return null;
        }

    }
}