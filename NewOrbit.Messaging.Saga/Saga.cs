namespace NewOrbit.Messaging.Saga
{
    public abstract class Saga<T> : ISaga where T: ISagaData
    {
        private T data;

        public void Initialise()
        {
            this.data = this.CreateData();
        }

        protected abstract T CreateData();

        public void Load(ISagaData sagaData)
        {
            this.data = (T) sagaData;
            this.SagaLoaded();
        }

        protected virtual void SagaLoaded()
        {
        }

        protected T Data => this.data;
    }
}
