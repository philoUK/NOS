using System;
using System.Threading.Tasks;
using MessagingFacts.Builders;
using MessagingFacts.Handlers;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
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
    }


}
