using MessagingFacts.Messages;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    public class OneOffSagaData : ISagaData
    {
        public string Id { get; set; }
    }

    public class OneOffSagaCommand : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id { get; set; }
    }

    public class OneOffSagaEvent : IEvent
    {
        public string CorrelationId { get; set; }
        public string Id { get; set; }
    }

    public class OneOffSaga : Saga<OneOffSagaData>, ISubscribeToEventsOf<OneOffEvent>,
        IPublishEventsOf<OneOffSagaEvent>, IHandleCommandsOf<OneOffCommand>
    {
        public OneOffSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        protected override OneOffSagaData CreateData(string id)
        {
            var results = new OneOffSagaData
            {
                Id = id
            };
            return results;
        }

        public void HandleEvent(OneOffEvent @event)
        {
            OneOffEventHandler.EventHandled = true;
            // submit a command
            // submit an event
            base.PublishEvent(new OneOffSagaEvent());
            base.SubmitCommand(new OneOffSagaCommand());
        }

        public void HandleCommand(OneOffCommand command)
        {
            OneOffCommandHandler.CommandHandled = true;
            // submit a command
            // submit an event
            base.PublishEvent(new OneOffSagaEvent());
            base.SubmitCommand(new OneOffSagaCommand());
        }
    }
}
