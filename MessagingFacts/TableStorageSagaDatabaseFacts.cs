using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class TableStorageSagaDatabaseFacts
    {
        [Fact]
        public async Task NewSagaShouldNotHaveItsDataInStorage()
        {
            var builder = new SagaDatabaseTestBuilder()
                .GivenABrandNewSaga()
                .WhenCheckingIfTheDataExists()
                .ThenTheDataShouldNotBeFound()
                .Cleanup();
            await builder.Execute().ConfigureAwait(false);
        }

        [Fact]
        public async Task SagaFoundAfterSaved()
        {
            var builder = new SagaDatabaseTestBuilder()
                .GivenABrandNewSaga()
                .WhenSaving()
                .WhenCheckingIfTheDataExists()
                .ThenTheDataShouldBeFound()
                .Cleanup();
            await builder.Execute().ConfigureAwait(false);
        }

        [Fact]
        public async Task SagaCorrectlyUpdates()
        {
            var builder = new SagaDatabaseTestBuilder()
                .GivenABrandNewSaga()
                .WhenSaving()
                .WhenChangingTheData()
                .ThenTheCurrentDataShouldBeDifferent()
                .Cleanup();
            await builder.Execute().ConfigureAwait(false);
        }

        [Fact]
        public async Task MissingSagaReturnsNull()
        {
            var builder = new SagaDatabaseTestBuilder()
                .GivenAnUnknownSagaId()
                .WhenFetchingSagaData()
                .ThenTheDataIsNull()
                .Cleanup();
            await builder.Execute().ConfigureAwait(false);
        }
    }
}
