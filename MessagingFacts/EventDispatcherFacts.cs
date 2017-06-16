using System.Threading.Tasks;
using MessagingFacts.Builders;
using Xunit;

namespace MessagingFacts
{
    public class EventDispatcherFacts
    {
        [Fact]
        public async Task OneOffSubscriberIsHandledViaDependencyFactory()
        {
            var builder = await new EventDispatcherBuilder()
                .GivenOneOffEventAndSubscriber()
                .Execute();
            builder.AssertSagaPathWasNotTaken();
            builder.AssertEventHandled();
        }

        [Fact]
        public async Task NewSagaHandlerIsHandledCorrectly()
        {
            var builder = await new EventDispatcherBuilder()
                .GivenOneOffEventAndNewSagaSubscriber()
                .Execute();
            builder.AssertNewSagaPathWasTaken();
            builder.AssertEventHandled();
        }

        [Fact]
        public async Task ExistingSagaHandlerIsHandledCorrectly()
        {
            var builder = await new EventDispatcherBuilder()
                .GivenOneOffEventAndExistingSagaSubscriber()
                .Execute();
            builder.AssertExistingSagaPathWasTaken();
            builder.AssertEventHandled();
        }
    }


}
