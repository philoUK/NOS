using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagingFacts.Sagas;
using Moq;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Timeouts;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class TimeoutDispatcherTestBuilder
    {
        private readonly Mock<ITimeoutDatabase> timeoutDatabase = new Mock<ITimeoutDatabase>();
        private readonly Mock<ISagaDatabase> sagaDatabase = new Mock<ISagaDatabase>();
        private readonly Mock<IClientCommandBus> cmdBus = new Mock<IClientCommandBus>();
        private readonly Mock<IEventBus> eventBus = new Mock<IEventBus>();
        private bool delayCancellation;
        private bool cancellationExceptionThrown;
        private bool checkCancellationExceptionThrown;
        private bool checkNoSagaCommandsAreSent;
        private bool checkNoSagaEventsArePublished;
        private bool checkTimeoutNotDeleted;
        private bool checkCommandsAreSent;
        private bool checkEventsArePublished;
        private bool checkTimeoutIsDeleted;

        public TimeoutDispatcherTestBuilder GivenNoTimeouts()
        {
            this.timeoutDatabase.Setup(db => db.GetExpiredTimeoutsSince(It.IsAny<DateTime>()))
                .Returns(Enumerable.Empty<TimeoutData>());
            return this;
        }

        public TimeoutDispatcherTestBuilder GivenADelay()
        {
            this.delayCancellation = true;
            return this;
        }


        public TimeoutDispatcherTestBuilder ThenThrowsTheAppropriateException()
        {
            this.checkCancellationExceptionThrown = true;
            return this;
        }

        public async Task Execute()
        {
            await Monitor().ConfigureAwait(false);
            this.Verify();
        }

        private async Task Monitor()
        {
            try
            {
                this.cancellationExceptionThrown = false;
                var sut = new TimeoutDispatcher(this.timeoutDatabase.Object, this.sagaDatabase.Object,
                    this.eventBus.Object,
                    this.cmdBus.Object);
                var token = new CancellationTokenSource(this.delayCancellation ? 500 : 10);
                await sut.Monitor(token.Token);
            }
            catch (OperationCanceledException)
            {
                this.cancellationExceptionThrown = true;
            }
            catch (ArgumentException)
            {
                
            }
        }

        private void Verify()
        {
            if (this.checkCancellationExceptionThrown)
            {
                Assert.True(this.cancellationExceptionThrown);
            }
            if (this.checkNoSagaCommandsAreSent)
            {
                this.cmdBus.Verify(bus => bus.Submit(It.IsAny<ICommand>()), Times.Never());
            }
            if (this.checkNoSagaEventsArePublished)
            {
                this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(),It.IsAny<IEvent>()), Times.Never());
            }
            if (this.checkTimeoutNotDeleted)
            {
                this.timeoutDatabase.Verify(db => db.Delete(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            }
            if (this.checkCommandsAreSent)
            {
                this.cmdBus.Verify(bus => bus.Submit(It.IsAny<ICommand>()), Times.AtLeastOnce());
            }
            if (this.checkEventsArePublished)
            {
                this.eventBus.Verify(bus => bus.Publish(It.IsAny<object>(), It.IsAny<IEvent>()), Times.AtLeastOnce());
            }
            if (this.checkTimeoutIsDeleted)
            {
                this.timeoutDatabase.Verify(db => db.Delete(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            }
        }

        public TimeoutDispatcherTestBuilder GivenATimeoutToASagaThatErrors()
        {
            var id = "id";
            var list = new List<TimeoutData>
            {
                new TimeoutData
                {
                    TargetId = id,
                    TargetMethod = "HandleTimeout",
                    TargetType = typeof(BadSaga).AssemblyQualifiedName,
                    Timeout = DateTime.UtcNow.AddMinutes(-5)
                }
            };
            this.timeoutDatabase.Setup(db => db.GetExpiredTimeoutsSince(It.IsAny<DateTime>()))
                .Returns(list);
            this.sagaDatabase.Setup(db => db.LoadSagaData(It.IsAny<string>()))
                .Returns(Task.FromResult((ISagaData)new BadSagaData {Id = id}));
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenNoSagaCommandsAreSent()
        {
            this.checkNoSagaCommandsAreSent = true;
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenNoSagaEventsArePublished()
        {
            this.checkNoSagaEventsArePublished = true;
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenTheTimeoutIsNotDeleted()
        {
            this.checkTimeoutNotDeleted = true;
            return this;
        }

        public TimeoutDispatcherTestBuilder GivenATimeoutToASagaThatSucceeds()
        {
            var id = "id";
            var list = new List<TimeoutData>
            {
                new TimeoutData
                {
                    TargetId = id,
                    TargetMethod = "OnTimeout",
                    TargetType = typeof(OneOffSaga).AssemblyQualifiedName,
                    Timeout = DateTime.UtcNow.AddMinutes(-5)
                }
            };
            this.timeoutDatabase.Setup(db => db.GetExpiredTimeoutsSince(It.IsAny<DateTime>()))
                .Returns(list);
            this.sagaDatabase.Setup(db => db.LoadSagaData(It.IsAny<string>()))
                .Returns(Task.FromResult((ISagaData) new OneOffSagaData {Id = id}));
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenSagaCommandsAreSent()
        {
            this.checkCommandsAreSent = true;
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenSagaEventsArePublished()
        {
            this.checkEventsArePublished = true;
            return this;
        }

        public TimeoutDispatcherTestBuilder ThenTheTimeoutIsDeleted()
        {
            this.checkTimeoutIsDeleted = true;
            return this;
        }
    }
}
