using System;
using System.Collections.Generic;
using System.Text;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    internal class BadSagaData : ISagaData
    {
        public string Id { get; set; }
    }

    internal class BadSaga : Saga<BadSagaData>
    {
        public BadSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        protected override BadSagaData CreateData(string id)
        {
            return new BadSagaData { Id = id };
        }

        private void HandleTimeout()
        {
            throw new ArgumentException();
        }
    }
}
