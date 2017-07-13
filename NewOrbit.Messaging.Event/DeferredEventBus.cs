using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Event
{
    public class DeferredEventBus : IEventBus
    {
        private IEventPublisherRegistry publisherRegistry;
        private readonly ILogEventBusMessages logger;
        private readonly IEventSubscriberRegistry subscriberRegistry;
        private readonly IDeferredEventMechanism mechanism;
        private List<Type> subscribers;

        public DeferredEventBus(IEventPublisherRegistry publisherRegistry, ILogEventBusMessages logger,
            IEventSubscriberRegistry subscriberRegistry, IDeferredEventMechanism mechanism)
        {
            this.publisherRegistry = publisherRegistry;
            this.logger = logger;
            this.subscriberRegistry = subscriberRegistry;
            this.mechanism = mechanism;
        }

        public async Task Submit(object publisher,IEvent @event)
        {
            this.EnsureThereIsASinglePublisher(@event);
            this.EnsurePublisherIsCorrect(publisher, @event);
            this.GetSubscribers(@event);
            await this.DispatchToEachSubscriber(@event).ConfigureAwait(false);
            this.LogIfNoSubscribersFound(@event);
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

        private void GetSubscribers(IEvent @event)
        {
            this.subscribers = this.subscriberRegistry.GetSubscribers(@event).ToList();

        }

        private void LogIfNoSubscribersFound(IEvent @event)
        {
            if (!this.subscribers.Any())
            {
                this.logger.NoSubscribersFoundForEvent(@event);
            }
        }

        private async Task DispatchToEachSubscriber(IEvent @event)
        {
            foreach (var subscriber in this.subscribers)
            {
                await this.mechanism.Defer(@event, subscriber).ConfigureAwait(false);
            }
        }

        public Task Publish(object publisher, IEvent @event)
        {
            return Submit(publisher, @event);
        }
    }
}