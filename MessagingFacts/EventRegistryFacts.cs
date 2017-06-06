using System;
using System.Collections.Generic;
using System.Linq;
using MessagingFacts.Messages;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Monitoring.Events;
using TestExtras;
using Xunit;

namespace MessagingFacts
{
    public class EventRegistryFacts
    {
        // finds none
        [Fact]
        public void DoesNotFindAnySubscriber()
        {
            var sut = new EventRegistry();
            Assert.False(sut.GetSubscribers(new OrphanedEvent()).Any());
        }


        // finds 1 in same assembly
        [Fact]
        public void FindsSubscriberInSameAssembly()
        {
            var sut = new EventRegistry();
            var results = sut.GetSubscribers(new CommandTestedEvent()).ToList();
            Assert.Equal(1, results.Count);
            Assert.True(results[0] == typeof(CommandTestedEventSubscriber));
        }

        // Finds 1 in different assembly
        [Fact]
        public void FindsSubscriberInDifferentAssembly()
        {
            var sut = new EventRegistry();
            var results = sut.GetSubscribers(new ExternalEvent()).ToList();
            Assert.Equal(1, results.Count);
            Assert.True(results[0] == typeof(ExternalEventSubscriber));
        }

        // finds no publisher
        [Fact]
        public void DoesNotFindAnyPublisher()
        {
            var sut = new EventRegistry();
            Assert.Null(sut.GetPublisher(new OrphanedEvent()));
        }

        // Finds publisher in same assembly
        [Fact]
        public void FindsPublisherInSameAssembly()
        {
            var sut = new EventRegistry();
            var result = sut.GetPublisher(new CommandTestedEvent());
            Assert.NotNull(result);
            Assert.True(result == typeof(CommandTestedEventPublisher));
        }

        [Fact]
        public void FindsPublisherInDifferentAssembly()
        {
            var sut = new EventRegistry();
            var result = sut.GetPublisher(new ExternalEvent());
            Assert.NotNull(result);
            Assert.True(result == typeof(ExternalEventPublisher));
        }

        [Fact]
        public void IdentifiesMultiplePublishersCorrectly()
        {
            var sut = new EventRegistry();
            Assert.Throws<MultipleEventPublishersFoundException>(() => sut.GetPublisher(new BadEvent()));
        }

        [Fact]
        public void FindsClassThatPublishesMoreThanOne()
        {
            var sut = new EventRegistry();
            var events = new List<IEvent>
            {
                new CommandCouldNotBeReadEvent(),
                new CommandDidNotDefineAHandlerEvent(),
                new CommandWasDispatchedEvent()
            };
            foreach (var @event in events)
            {
                var publisher = sut.GetPublisher(@event);
                Assert.True(publisher == typeof(ReceivingCommandBus));
            }
        }
    }
}
