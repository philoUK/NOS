using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    internal class OrphanedEvent : IEvent
    {
        public string Id => Guid.NewGuid().ToString();
    }
}
