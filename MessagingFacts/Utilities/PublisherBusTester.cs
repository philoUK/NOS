using System;
using System.Collections.Generic;
using Moq;
using NewOrbit.Messaging;

namespace MessagingFacts.Utilities
{
    internal class PublisherBusTester
    {
        private readonly Dictionary<Type,List<Type>> subscribers = new Dictionary<Type, List<Type>>();
        private readonly Mock<IGetEventSubscribers> fetcher = new Mock<IGetEventSubscribers>();

        public PublisherBusTester GivenSubscriber<T, T1>()
        {
            var key = typeof(T);
            if (!this.subscribers.ContainsKey(key))
            {
                this.subscribers.Add(key, new List<Type>());
            }
            subscribers[key].Add(typeof(T1));
            return this;
        }

        public void Execute(IEvent @event)
        {
            this.fetcher.Setup(f => f.GetSubscribers(It.IsAny<IEvent>()))
                .Returns(this.GetSubscribers(@event));
            var bus = new PublisherBus(this.fetcher.Object);
            bus.Publish(@event);
        }

        private IEnumerable<IRespondToEvents> GetSubscribers(IEvent @event)
        {
            var key = @event.GetType();
            if (this.subscribers.ContainsKey(key))
            {
                foreach (var subscriberType in this.subscribers[key])
                {
                    yield return (IRespondToEvents) Activator.CreateInstance(subscriberType);
                }
            }
        }

        public PublisherBusTester GivenNoSubscribersForEvent<T>()
        {
            var key = typeof(T);
            if (this.subscribers.ContainsKey(key))
            {
                this.subscribers[key].Clear();
            }
            return this;
        }
    }
}
