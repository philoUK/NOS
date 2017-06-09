using System;
using NewOrbit.Messaging.Saga;

namespace MessagingFacts.Sagas
{
    internal class TestSagaData : ISagaData
    {
        
    }

    internal class TestSaga : ISaga
    {
        private ISagaData data;

        public bool InitialiseCalled { get; set; }
        public void Initialise()
        {
            this.InitialiseCalled = true;
        }

        public void Load(ISagaData sagaData)
        {
            this.data = sagaData;
        }

        internal bool LoadCalledWith(TestSagaData sagaData)
        {
            return this.data?.Equals(sagaData) ?? false;
        }
    }
}
