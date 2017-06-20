using System;
using System.Linq;
using System.Reflection;
using NewOrbit.Messaging.Saga.Commands;

namespace NewOrbit.Messaging.Saga
{
    public abstract class Saga<T> : ISaga where T: ISagaData
    {
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        protected Saga(IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public string SagaId
        {
            get
            {
                if (this.Data == null)
                {
                    return "";
                }
                return this.Data.Id;
            }
        }

        public ISagaData SagaData => this.Data;

        public void Initialise(string id)
        {
            this.Data = this.CreateData(id);
        }

        protected abstract T CreateData(string id);

        public void Load(ISagaData sagaData)
        {
            this.Data = (T) sagaData;
            this.SagaLoaded();
        }

        protected virtual void SagaLoaded()
        {
        }

        protected T Data { get; private set; }

        protected void PublishEvent(IEvent @event)
        {
            this.eventBus.Publish(this, @event).Wait();
        }

        protected void SubmitCommand(ICommand command)
        {
            this.commandBus.Submit(command).Wait();
        }

        protected void RegisterTimeout(string methodName, TimeSpan timeToResponsd)
        {
            if (this.IsMethodAvailable(methodName))
            {
                var msg = new RegisterTimeoutCommand
                {
                    CorrelationId = this.SagaId,
                    Id = Guid.NewGuid().ToString(),
                    MethodName = methodName,
                    Timeout = DateTime.UtcNow.Add(timeToResponsd)
                };
                this.commandBus.Submit(msg).Wait();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Could not register a timeout for class {this.GetType().Name} due to missing handler method {methodName}");
            }
        }

        private bool IsMethodAvailable(string methodName)
        {
            return this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
