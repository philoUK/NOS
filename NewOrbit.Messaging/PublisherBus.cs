using System.Linq;
using NewOrbit.Messaging.Abstractions;
using NewOrbit.Messaging.Exceptions;

namespace NewOrbit.Messaging
{
    public class PublisherBus
    {
        private readonly IGetEventSubscribers subscriberFetcher;
        private readonly IGetEventPublishers publisherFetcher;
        private readonly IPublisherBusLogger logger;

        public PublisherBus(IGetEventSubscribers subscriberFetcher, IGetEventPublishers publisherFetcher,
            IPublisherBusLogger logger)
        {
            this.subscriberFetcher = subscriberFetcher;
            this.publisherFetcher = publisherFetcher;
            this.logger = logger;
        }

        public void Publish(object publisher, IEvent @event)
        {
            this.EnsureThereIsOnlyASinglePublisher(publisher, @event);
            this.EnsurePublisherIsAuthorised(publisher, @event);
            foreach (var subscriber in this.subscriberFetcher.GetSubscribers(@event))
            {
                subscriber.Respond(@event);
            }
        }

        private void EnsurePublisherIsAuthorised(object publisher, IEvent @event)
        {
            var publisherType = this.publisherFetcher.GetPublishers(@event).Single();
            if (publisher.GetType() != publisherType)
            {
                this.logger.LogUnregisteredPublisherException(publisher,@event);
                throw new UnregisteredPublisherException(publisher, @event);
            }
        }

        private void EnsureThereIsOnlyASinglePublisher(object publisher, IEvent @event)
        {
            var publishers = this.publisherFetcher.GetPublishers(@event).ToList();
            if (publishers.Count == 0)
            {
                this.logger.LogNoPublisherFoundException(@event);
                throw new NoPublisherDefinedException(publisher, @event);
            }
            if (publishers.Count > 1)
            {
                this.logger.LogTooManyPublishersFoundException(@event);
                throw new MultiplePublishersDefinedException(@event);
            }
        }
    }
}
