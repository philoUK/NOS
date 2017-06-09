using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Sagas
{
    internal class CreateSagaCommand : ICommand
    {
        public CreateSagaCommand()
        {
            this.CorrelationId = this.Id = Guid.NewGuid().ToString();
        }

        public string CorrelationId { get; }
        public string Id { get; }
    }
}
