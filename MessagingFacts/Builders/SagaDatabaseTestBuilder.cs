using System.Threading.Tasks;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging;
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

        public SagaDatabaseTestBuilder()
        {
            this.config = new Mock<ISagaDatabaseConfig>();
            this.config.SetupGet(c => c.ConnectionString).Returns("UseDevelopmentStorage=true");
            this.config.SetupGet(c => c.TableName).Returns("testsagadatabase");
        }

        public SagaDatabaseTestBuilder GivenABrandNewSaga()
        {
            this.saga = new TestSaga(new Mock<IClientCommandBus>().Object, new Mock<IEventBus>().Object);
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
            if (this.checkExists)
            {
                await this.ExecuteCheckExists().ConfigureAwait(false);
            }
            if (this.ensureNotFound)
            {
                this.EnsureNotFound();
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

        private void EnsureNotFound()
        {
            Assert.False(this.exists);
        }

        private async Task ExecuteCleanup()
        {
            var sut = new TableStorageSagaDatabase(this.config.Object);
            await sut.DeleteSagaData(this.saga.SagaId).ConfigureAwait(false);
        }
    }
}
