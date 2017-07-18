using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Shared;
using Serilog;

namespace WebJob
{
    class EventDistributor
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAzureStorageQueueConfig config;
        private readonly IEventSubscriberRegistry registry;
        private readonly IEventBus eventBus;
        private readonly IDeferredEventMechanism eventMechanism;

        public EventDistributor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.config = this.serviceProvider.GetService<IAzureStorageQueueConfig>();
            this.registry = this.serviceProvider.GetService<IEventSubscriberRegistry>();
            this.eventBus = this.serviceProvider.GetService<IEventBus>();
            this.eventMechanism = this.serviceProvider.GetService<IDeferredEventMechanism>();
        }

        public async Task Process(CancellationToken token)
        {
            var queues = new List<string>();
            foreach (var tuple in ReflectionHelpers.TypesThatImplementInterface(t => t == typeof(ISubscribeToEventsOf<>),
                "NewOrbit.Messaging"))
            {
                var cmdType = tuple.Item1;
                var queue = config.EventQueue(cmdType);
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
            Log.Logger.Information($"About to start watching Event queue {queueName}");
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
                        var @event = msg.ExtractEventWrapper();
                        Log.Logger.Information($"Found an event of type {@event.EventType} in the Queue {queueName}");
                        var bus = new ReceivingEventBus(this.eventBus, this.registry, this.eventMechanism);
                        await bus.Dispatch(@event).ConfigureAwait(false);
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