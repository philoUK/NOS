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
        //[Fact]
        //public async Task NewIMessageShouldNotBeFound()
        //{
        //    var builder = new SagaDatabaseTestBuilder()
        //        .GivenANewMessage();
        //    Assert.True(await builder.NewMessageIsNotFound());
        //}

        //[Fact]
        //public async Task MessageShouldBeFoundAfterSaving()
        //{
        //    var builder = await new SagaDatabaseTestBuilder()
        //        .GivenAnExistingMessage();
        //    Assert.True(await builder.MessageIsFound());
        //    await builder.DeleteMessage();
        //}

        //[Fact]
        //public async Task TaskSavesAndLoadsCorrectly()
        //{
        //    var builder = await new SagaDatabaseTestBuilder()
        //        .GivenASaga()
        //        .WhenSavingTheSaga()
        //        .ThenTheSagaCanBeCorrectlyLoaded()
        //        .DeleteSaga();
        //}
    }
}
