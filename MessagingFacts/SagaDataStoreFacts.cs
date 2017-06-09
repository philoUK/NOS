using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class SagaDataStoreFacts
    {
        // when not found, the saga type is created, and asked to initialise itself, and is stored in the data store
        [Fact]
        public async Task NewSagaIsInitialisedAndStored()
        {
            var builder = await new SagaDataStoreTestBuilder()
                .GivenNoPreviousDataForSaga()
                .Create();
            builder.AssertSagaInitialisedItself();
            builder.AssertSagaWasPersistedToDatabase();
        }

        [Fact]
        public async Task OldSagaHasItsDataLoadedBackIn()
        {
            var builder = await new SagaDataStoreTestBuilder()
                .GivenPreviousDataForSaga()
                .Create();
            builder.AssertSagaLoadedItsData();
        }
    }

    
}
