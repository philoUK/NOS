using System;
using System.Collections.Generic;
using System.Linq;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Event
{
    public class EventRegistry : IEventSubscriberRegistry, IEventPublisherRegistry
    {
        private readonly Dictionary<Type,List<Type>> cachedSubscribers = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, List<Type>> cachedPublishers = new Dictionary<Type, List<Type>>();
        public EventRegistry()
        {
            CacheSubscribers();
            CachePublishers();
        }

        private void CacheSubscribers()
        {
            foreach (var tuple in
                ReflectionHelpers.TypesThatImplementInterface(ti => ti == typeof(ISubscribeToEventsOf<>), "NewOrbit.Messaging"))
            {
                var eventType = tuple.Item1;
                var handlerType = tuple.Item2;
                if (!this.cachedSubscribers.ContainsKey(eventType))
                {
                    this.cachedSubscribers.Add(eventType, new List<Type>());
                }
                this.cachedSubscribers[eventType].Add(handlerType);
            }
        }

        private void CachePublishers()
        {
            foreach (var tuple in
                ReflectionHelpers.TypesThatImplementInterface(ti => ti == typeof(IPublishEventsOf<>), "NewOrbit.Messaging"))
            {
                var eventType = tuple.Item1;
                var handlerType = tuple.Item2;
                if (!this.cachedPublishers.ContainsKey(eventType))
                {
                    this.cachedPublishers.Add(eventType, new List<Type>());
                }
                this.cachedPublishers[eventType].Add(handlerType);
            }
        }

        public IEnumerable<Type> GetSubscribers(IEvent @event)
        {
            var key = @event.GetType();
            if (this.cachedSubscribers.ContainsKey(key))
            {
                return this.cachedSubscribers[key];
            }
            return Enumerable.Empty<Type>();
        }

        public Type GetPublisher(IEvent @event)
        {
            var key = @event.GetType();
            if (this.cachedPublishers.ContainsKey(key))
            {
                if (this.cachedPublishers[key].Count > 1)
                {
                    throw new MultipleEventPublishersFoundException(@event);
                }
                return this.cachedPublishers[key].Single();
            }
            return null;
        }
    }
}
