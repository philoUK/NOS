using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class TableStorageTimeoutDatabaseFacts
    {
        [Fact]
        public void TimeoutRequestStoredProperly()
        {
            var builder = new TimeoutDatabaseTestBuilder()
                .GivenATimeoutRequest()
                .Save();
            builder.VerifyMessageStoredInTheCorrectQueue();
            builder.VerifyMessageFormattedProperly();
        }

        [Fact]
        public void TimeoutRequestDeletedProperly()
        {
            var builder = new TimeoutDatabaseTestBuilder()
                .GivenATimeoutRequest()
                .Save()
                .Delete();
            builder.VerifyMessageNotFound();
        }
    }
}
