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
                if (this.Data != null)
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
    }
}
