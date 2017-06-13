using System.Threading.Tasks;
using MessagingFacts.Handlers;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class SagaDataStoreTestBuilder
    {
        private Mock<ISagaDatabase> database = new Mock<ISagaDatabase>();
        private TestSaga saga;
        private TestSagaData sagaData;
       
        public SagaDataStoreTestBuilder GivenNoPreviousDataForSaga()
        {
            this.database.Setup(db => db.SagaExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            return this;
        }

        public async Task<SagaDataStoreTestBuilder> Create()
        {
            var sut = new SagaDataStore(this.database.Object, new FakeHandlerFactory());
            this.saga = (TestSaga)await sut.Make(typeof(TestSaga), new CreateSagaCommand()).ConfigureAwait(false);
            return this;
        }

        public void AssertSagaInitialisedItself()
        {
            Assert.True(this.saga.InitialiseCalled);
        }

        public void AssertSagaWasPersistedToDatabase()
        {
            this.database.Verify(db => db.Save(It.IsAny<ISaga>()), Times.Once());
        }

        public SagaDataStoreTestBuilder GivenPreviousDataForSaga()
        {
            this.database.Setup(db => db.SagaExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            this.sagaData = new TestSagaData();
            this.database.Setup(db => db.LoadSagaData(It.IsAny<string>()))
                .Returns(Task.FromResult((ISagaData) this.sagaData));
            return this;
        }

        public void AssertSagaLoadedItsData()
        {
            Assert.True(this.saga.LoadCalledWith(this.sagaData));
        }

    }
}
