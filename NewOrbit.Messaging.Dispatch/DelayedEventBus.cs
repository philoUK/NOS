using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Dispatch
{
    internal class DelayedEventBus : IEventBus
    {
        private readonly List<IEvent> delayedEvents = new List<IEvent>();

        public Task Publish(object publisher, IEvent @event)
        {
            this.delayedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public IEnumerable<IEvent> DelayedEvents => this.delayedEvents;
    }
}
