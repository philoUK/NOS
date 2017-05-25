using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Abstractions;

namespace MessagingFacts.Utilities
{
    internal class FakeEvent : IEvent
    {
        public Guid UniqueIdentifier => Guid.NewGuid();
        public bool HandledBySubscriberOne { get; set; }
        public bool HandledBySubscriberTwo { get; set; }
    }

    internal class FakeEventSubscriberOne : ISubscribeToEventOf<FakeEvent>
    {
        public void HandleEvent(FakeEvent @event)
        {
            @event.HandledBySubscriberOne = true;
        }

        public void Respond(IEvent @event)
        {
            this.HandleEvent((FakeEvent)@event);
        }
    }

    internal class FakeEventSubscriberTwo : ISubscribeToEventOf<FakeEvent>
    {
        public void HandleEvent(FakeEvent @event)
        {
            @event.HandledBySubscriberTwo = true;
        }

        public void Respond(IEvent @event)
        {
            this.HandleEvent((FakeEvent)@event);
        }
    }
}
