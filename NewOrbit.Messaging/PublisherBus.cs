namespace NewOrbit.Messaging
{
    public class PublisherBus
    {
        private IGetEventSubscribers @subscriberFetcher;

        public PublisherBus(IGetEventSubscribers @subscriberFetcher)
        {
            this.subscriberFetcher = subscriberFetcher;
        }

        public void Publish(IEvent @event)
        {
            foreach (var subscriber in this.subscriberFetcher.GetSubscribers(@event))
            {
                subscriber.Respond(@event);
            }
        }
    }
}
