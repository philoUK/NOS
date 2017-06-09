using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    internal class OrphanedEvent : IEvent
    {
        public string CorrelationId { get; set; }
        public string Id => Guid.NewGuid().ToString();
    }
}
