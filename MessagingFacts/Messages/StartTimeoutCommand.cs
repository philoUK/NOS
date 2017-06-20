using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    public class StartTimeoutCommand : ICommand
    {
        public StartTimeoutCommand()
        {
            this.CorrelationId = this.Id = Guid.NewGuid().ToString();
        }

        public string CorrelationId { get; }
        public string Id { get; }
    }
}
