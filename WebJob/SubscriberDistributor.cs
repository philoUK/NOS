

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using Serilog;

namespace WebJob
{
    class SubscriberDistributor
    {
        private readonly IAzureStorageQueueConfig config;
        private readonly IServiceProvider serviceProvider;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public SubscriberDistributor(IServiceProvider serviceProvider)
        {
            this.config = serviceProvider.GetService<IAzureStorageQueueConfig>();
            this.dependencyFactory = serviceProvider.GetService<IDependencyFactory>();
            this.sagaDatabase = serviceProvider.GetService<ISagaDatabase>();
            this.commandBus = serviceProvider.GetService<IClientCommandBus>();
            this.eventBus = serviceProvider.GetService<IEventBus>();
        }

        public async Task Process(CancellationToken token)
        {
            var queues = new List<string>();
            foreach (var tuple in ReflectionHelpers.TypesThatImplementInterface(t => t == typeof(ISubscribeToEventsOf<>),
                "NewOrbit.Messaging"))
            {
                var evtType = tuple.Item1;
                var queue = config.EventSubscriberQueue(evtType);
                if (!queues.Contains(queue))
                {
                    queues.Add(queue);
                }
            }
            var tasks = queues.Select(WatchQueue);
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task WatchQueue(string queueName)
        {
            Log.Logger.Information($"About to start watching Event Subscription Queue {queueName}");
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            var waitTime = TimeSpan.FromSeconds(0);
            while (true)
            {
                CloudQueueMessage msg;
                do
                {
                    msg = await queue.GetMessageAsync().ConfigureAwait(false);
                    if (msg != null)
                    {
                        waitTime = TimeSpan.FromSeconds(-1);
                        var esw = msg.ExtractEventSubscriptionWrapper();
                        var @event = esw.ExtractEvent();
                        var subscriberType = Type.GetType(esw.SubscriberType);
                        var dispatcher = new EventDispatcher(@event, subscriberType, dependencyFactory, sagaDatabase,
                            commandBus, eventBus);
                        await dispatcher.Dispatch().ConfigureAwait(false);
                        await queue.DeleteMessageAsync(msg).ConfigureAwait(false);
                    }
                } while (msg != null);
                waitTime = waitTime.Add(TimeSpan.FromSeconds(1));
                if (waitTime > TimeSpan.FromMinutes(1))
                {
                    waitTime = TimeSpan.FromMinutes(1);
                }
                await Task.Delay(waitTime).ConfigureAwait(false);
            }
        }
    }
}
