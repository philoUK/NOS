using MessagingFacts.Sagas;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using Xunit;

namespace MessagingFacts
{
    public class ReflectionHelperFacts
    {
        [Fact]
        public void NonSagaTypeIsNotASaga()
        {
            Assert.False(typeof(TestSagaData).IsSubClassOfGenericType(typeof(Saga<>)));
        }

        [Fact]
        public void SagaTypeIsASaga()
        {
            Assert.True(typeof(TestSaga).IsSubClassOfGenericType(typeof(Saga<>)));
        }
    }
}
