using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    internal class TestSagaData : ISagaData
    {
        
    }

    internal class TestSaga : Saga<TestSagaData>
    {
        public bool InitialiseCalled { get; set; }
        private ISagaData testSagaData;

        protected override TestSagaData CreateData()
        {
            this.InitialiseCalled = true;
            return new TestSagaData();
        }

        protected override void SagaLoaded()
        {
            this.testSagaData = this.Data;
        }

        public bool LoadCalledWith(TestSagaData sagaData)
        {
            return this.testSagaData?.Equals(sagaData) ?? false;
        }
    }

    //internal class TestSaga : ISaga
    //{
    //    private ISagaData data;

    //    public bool InitialiseCalled { get; set; }
    //    public void Initialise()
    //    {
    //        this.InitialiseCalled = true;
    //    }

    //    public void Load(ISagaData sagaData)
    //    {
    //        this.data = sagaData;
    //    }

    //    internal bool LoadCalledWith(TestSagaData sagaData)
    //    {
    //        return this.data?.Equals(sagaData) ?? false;
    //    }
    //}
}
