using System;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    internal class TestSagaData : ISagaData
    {
        public TestSagaData()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; }
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

}
