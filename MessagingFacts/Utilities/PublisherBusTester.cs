using System;
using System.Collections.Generic;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Abstractions;
using NewOrbit.Messaging.Exceptions;
using Xunit;

namespace MessagingFacts.Utilities
{
    internal class PublisherBusTester
    {
        private readonly Dictionary<Type,List<Type>> subscribers = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type,List<Type>> publishers = new Dictionary<Type, List<Type>>();
        private readonly Mock<IGetEventSubscribers> fetcher = new Mock<IGetEventSubscribers>();
        private readonly Mock<IGetEventPublishers> publisherFetcher = new Mock<IGetEventPublishers>();
        private bool noPublishersFoundThrown = false;
        private bool tooManyPublishersFoundThrown = false;
        private bool unregisteredPublisherThrown = false;
        private readonly Mock<IPublisherBusLogger> logger = new Mock<IPublisherBusLogger>();

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

        public void Execute(object publisher,IEvent @event)
        {
            SetUpMocks(@event);
            PublishEvent(publisher, @event);
        }

        private void SetUpMocks(IEvent @event)
        {
            this.fetcher.Setup(f => f.GetSubscribers(It.IsAny<IEvent>()))
                .Returns(this.GetSubscribers(@event));
            this.publisherFetcher.Setup(f => f.GetPublishers(It.IsAny<IEvent>()))
                .Returns(this.GetPublishers(@event));
        }

        private void PublishEvent(object publisher, IEvent @event)
        {
            var bus = new PublisherBus(this.fetcher.Object, this.publisherFetcher.Object, this.logger.Object);

            try
            {
                bus.Publish(publisher, @event);
            }
            catch (NoPublisherDefinedException)
            {
                noPublishersFoundThrown = true;
            }
            catch (MultiplePublishersDefinedException)
            {
                tooManyPublishersFoundThrown = true;
            }
            catch (UnregisteredPublisherException)
            {
                unregisteredPublisherThrown = true;
            }
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

        private IEnumerable<Type> GetPublishers(IEvent @event)
        {
            var key = @event.GetType();
            if (this.publishers.ContainsKey(key))
            {
                foreach (var publisherType in this.publishers[key])
                {
                    yield return publisherType;
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

        public PublisherBusTester GivenPublisher<T, T1>()
        {
            var key = typeof(T);
            if (!this.publishers.ContainsKey(key))
            {
                this.publishers.Add(key, new List<Type>());
            }
            this.publishers[key].Add(typeof(T1));
            return this;
        }

        public void AssertNoPublisherFoundErrorThrownAndLogged()
        {
            Assert.True(noPublishersFoundThrown);
            this.logger.Verify(l => l.LogNoPublisherFoundException(It.IsAny<IEvent>()), Times.AtLeastOnce());
        }

        public void AssertTooManyPublishersFoundErrorThrownAndLogged()
        {
            Assert.True(tooManyPublishersFoundThrown);
            this.logger.Verify(l => l.LogTooManyPublishersFoundException(It.IsAny<IEvent>()), Times.AtLeastOnce());
        }

        public void AssertUnregisteredPublisherErrorThrownAndLogged()
        {
            Assert.True(unregisteredPublisherThrown);
            this.logger.Verify(l => l.LogUnregisteredPublisherException(It.IsAny<object>(), It.IsAny<IEvent>()), Times.AtLeastOnce());
        }
    }
}
