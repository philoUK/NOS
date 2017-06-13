namespace NewOrbit.Messaging.Saga
{
    public abstract class Saga<T> : ISaga where T: ISagaData
    {
        public string SagaId => this.Data?.Id ?? "";
        public ISagaData SagaData => this.Data;

        public void Initialise()
        {
            this.Data = this.CreateData();
        }

        protected abstract T CreateData();

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
