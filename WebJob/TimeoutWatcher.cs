using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using NewOrbit.Messaging.Timeouts;
using Serilog;

namespace WebJob
{
    class TimeoutWatcher
    {
        private readonly ITimeoutDatabase timeoutDatabase;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public TimeoutWatcher(IServiceProvider serviceProvider)
        {
            this.timeoutDatabase = serviceProvider.GetService<ITimeoutDatabase>();
            this.dependencyFactory = serviceProvider.GetService<IDependencyFactory>();
            this.sagaDatabase = serviceProvider.GetService<ISagaDatabase>();
            this.commandBus = serviceProvider.GetService<IClientCommandBus>();
            this.eventBus = serviceProvider.GetService<IEventBus>();
        }

        public async Task Process(CancellationToken token)
        {
            Log.Logger.Information("About to start watching time out requiests");
            var waitTime = TimeSpan.FromSeconds(0);
            while (true)
            {
                foreach (var dataItem in this.timeoutDatabase.GetExpiredTimeoutsSince(DateTime.UtcNow))
                {
                    waitTime = TimeSpan.FromSeconds(-1);
                    var dispatcher = new TimeoutDispatcher(dataItem, this.dependencyFactory, this.sagaDatabase,
                        this.commandBus, this.eventBus);
                    await dispatcher.Dispatch().ConfigureAwait(false);
                    this.timeoutDatabase.Delete(dataItem);
                }
                waitTime += TimeSpan.FromSeconds(1);
                if (waitTime > TimeSpan.FromMinutes(1))
                {
                    waitTime = TimeSpan.FromMinutes(1);
                }
                await Task.Delay(waitTime, token).ConfigureAwait(false);
            }

        }
    }
}