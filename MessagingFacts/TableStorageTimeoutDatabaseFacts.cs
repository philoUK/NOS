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
    }
}
