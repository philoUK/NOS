using System;
using System.Threading.Tasks;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Saga.Azure;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class SagaDatabaseTestBuilder
    {
        private readonly Mock<ISagaDatabaseConfig> config;

        private TestSaga saga;
        private bool exists;
        private bool checkExists;
        private bool ensureNotFound;
        private bool cleanupAfterwards;
        private bool ensureSave;
        private bool ensureFound;
        private bool changeTheData;
        private bool compareData;
        private string sagaId;
        private bool fetchSagaData;
        private bool checkForNullSaga;
        private ISagaData sagaData;

        public SagaDatabaseTestBuilder()
        {
            this.config = new Mock<ISagaDatabaseConfig>();
            this.config.SetupGet(c => c.ConnectionString).Returns("UseDevelopmentStorage=true");
            this.config.SetupGet(c => c.TableName).Returns("testsagadatabase");
        }

        public SagaDatabaseTestBuilder GivenABrandNewSaga()
        {
            this.saga = this.MakeTestSaga();
            this.saga.Initialise(Guid.NewGuid().ToString());
            return this;
        }

        public SagaDatabaseTestBuilder WhenCheckingIfTheDataExists()
        {
            this.checkExists = true;
            return this;
        }

        public SagaDatabaseTestBuilder ThenTheDataShouldNotBeFound()
        {
            this.ensureNotFound = true;
            return this;
        }

        public SagaDatabaseTestBuilder Cleanup()
        {
            this.cleanupAfterwards = true;
            return this;
        }

        public async Task Execute()
        {
            if (this.ensureSave)
            {
                await this.ExecuteSave().ConfigureAwait(false);
            }
            if (this.checkExists)
            {
                await this.ExecuteCheckExists().ConfigureAwait(false);
            }
            if (this.fetchSagaData)
            {
                await this.ExecuteFetchSagaData().ConfigureAwait(false);
            }
            if (this.ensureNotFound)
            {
                this.ExecuteEnsureNotFound();
            }
            if (this.ensureFound)
            {
                this.ExecuteEnsureFound();
            }
            if (this.checkForNullSaga)
            {
                this.ExecuteCheckForNullSaga();
            }
            if (this.changeTheData)
            {
                await this.ExecuteChangeData().ConfigureAwait(false);
            }
            if (this.compareData)
            {
                await this.ExecuteCompareData().ConfigureAwait(false);
            }
            if (this.cleanupAfterwards)
            {
                await this.ExecuteCleanup().ConfigureAwait(false);
            }
        }

        private async Task ExecuteCheckExists()
        {
            var sut = new TableStorageSagaDatabase(this.config.Object);
            this.exists = await sut.SagaExists(this.saga.SagaId).ConfigureAwait(false);
        }

        private void ExecuteEnsureNotFound()
        {
            Assert.False(this.exists);
        }

        private void ExecuteEnsureFound()
        {
            Assert.True(this.exists);
        }

        private async Task ExecuteCleanup()
        {
            if (this.saga == null)
            {
                return;
            }
            var sut = new TableStorageSagaDatabase(this.config.Object);
            await sut.DeleteSagaData(this.saga.SagaId).ConfigureAwait(false);
        }

        public SagaDatabaseTestBuilder WhenSaving()
        {
            this.ensureSave = true;
            return this;
        }

        public SagaDatabaseTestBuilder ThenTheDataShouldBeFound()
        {
            this.ensureFound = true;
            return this;
        }

        private async Task ExecuteSave()
        {
            var sut = new TableStorageSagaDatabase(this.config.Object);
            await sut.Save(this.saga).ConfigureAwait(false);
        }

        public SagaDatabaseTestBuilder WhenChangingTheData()
        {
            this.changeTheData = true;
            return this;
        }

        public SagaDatabaseTestBuilder ThenTheCurrentDataShouldBeDifferent()
        {
            this.compareData = true;
            return this;
        }

        private async Task ExecuteChangeData()
        {
            var changedSaga = this.MakeTestSaga();
            changedSaga.Load(new TestSagaData
            {
                Id = this.saga.SagaId,
                SomeField = "changed"
            });
            var sut = new TableStorageSagaDatabase(this.config.Object);
            await sut.Save(changedSaga).ConfigureAwait(false);
        }

        private async Task ExecuteCompareData()
        {
            var sut = new TableStorageSagaDatabase(this.config.Object);
            var newSaga = this.MakeTestSaga();
            newSaga.Load(await sut.LoadSagaData(this.saga.SagaId).ConfigureAwait(false));
            Assert.Equal(newSaga.SagaId, this.saga.SagaId);
            Assert.NotEqual(newSaga.SomeField, this.saga.SomeField);
        }

        private TestSaga MakeTestSaga()
        {
            return new TestSaga(new Mock<IClientCommandBus>().Object, new Mock<IEventBus>().Object);
        }

        public SagaDatabaseTestBuilder GivenAnUnknownSagaId()
        {
            this.sagaId = Guid.NewGuid().ToString();
            return this;
        }

        public SagaDatabaseTestBuilder WhenFetchingSagaData()
        {
            this.fetchSagaData = true;
            return this;
        }

        public SagaDatabaseTestBuilder ThenTheDataIsNull()
        {
            this.checkForNullSaga = true;
            return this;
        }

        private async Task ExecuteFetchSagaData()
        {
            var sut = new TableStorageSagaDatabase(this.config.Object);
            this.sagaData = await sut.LoadSagaData(this.sagaId).ConfigureAwait(false);
        }

        private void ExecuteCheckForNullSaga()
        {
            Assert.Null(this.sagaData);
        }
    }
}
