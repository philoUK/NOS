using System;
using MessagingFacts.Messages;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    public class TimingSagaData : ISagaData
    {
        public string Id { get; set; }
    }

    public class TimingSaga : Saga<TimingSagaData>, IHandleCommandsOf<StartTimeoutCommand>
    {
        public TimingSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        protected override TimingSagaData CreateData(string id)
        {
            return new TimingSagaData {Id = id};
        }

        public void OnTimeout()
        {
            throw new NotImplementedException();
        }

        public void HandleCommand(StartTimeoutCommand command)
        {
            base.RegisterTimeout(nameof(OnTimeout), TimeSpan.FromMinutes(10));
        }
    }

    public class InvalidTimingSaga : Saga<TimingSagaData>, IHandleCommandsOf<StartTimeoutCommand>
    {
        public InvalidTimingSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        protected override TimingSagaData CreateData(string id)
        {
            return new TimingSagaData {Id = id};
        }

        public void HandleCommand(StartTimeoutCommand command)
        {
            base.RegisterTimeout("Invalid", TimeSpan.FromMinutes(10));
        }
    }
}
