using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    internal class TestSagaData : ISagaData
    {
        public string Id { get; set; }
        public string SomeField { get; set; }
    }

    internal class TestSaga : Saga<TestSagaData>
    {
        public TestSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        public bool InitialiseCalled { get; set; }

        public string SomeField
        {
            get => this.Data.SomeField;
            set => this.Data.SomeField = value;
        }

        private ISagaData testSagaData;

        protected override TestSagaData CreateData(string id)
        {
            this.InitialiseCalled = true;
            return new TestSagaData {Id = id};
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
