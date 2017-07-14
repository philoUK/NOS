using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Event
{
    public class DeferredEventBus : IEventBus
    {
        private readonly IEventPublisherRegistry publisherRegistry;
        private readonly IDeferredEventMechanism mechanism;
        private List<Type> subscribers;

        public DeferredEventBus(IEventPublisherRegistry publisherRegistry, IDeferredEventMechanism mechanism)
        {
            this.publisherRegistry = publisherRegistry;
            this.mechanism = mechanism;
        }

        public async Task Submit(object publisher,IEvent @event)
        {
            this.EnsureThereIsASinglePublisher(@event);
            this.EnsurePublisherIsCorrect(publisher, @event);
            await this.mechanism.Defer(@event).ConfigureAwait(false);
        }

        private void EnsureThereIsASinglePublisher(IEvent @event)
        {
            var publishingType = this.publisherRegistry.GetPublisher(@event);
            if (publishingType == null)
            {
                throw new NoEventPublisherFoundException(@event);
            }
        }

        private void EnsurePublisherIsCorrect(object publisher, IEvent @event)
        {
            var publishingType = this.publisherRegistry.GetPublisher(@event);
            if (publishingType != publisher.GetType())
            {
                throw new UnauthorizedEventPublisherException(publisher, @event);
            }
        }


        public Task Publish(object publisher, IEvent @event)
        {
            return Submit(publisher, @event);
        }
    }
}