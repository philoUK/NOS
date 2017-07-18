using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Dispatch;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Shared;
using Serilog;

namespace WebJob
{
    class CommandWatcher
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAzureStorageQueueConfig config;
        private readonly ICommandHandlerRegistry registry;
        private readonly IDependencyFactory dependencyFactory;
        private readonly ISagaDatabase sagaDatabase;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public CommandWatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.config = this.serviceProvider.GetService<IAzureStorageQueueConfig>();
            this.registry = this.serviceProvider.GetService<ICommandHandlerRegistry>();
            this.dependencyFactory = this.serviceProvider.GetService<IDependencyFactory>();
            this.sagaDatabase = this.serviceProvider.GetService<ISagaDatabase>();
            this.commandBus = this.serviceProvider.GetService<IClientCommandBus>();
            this.eventBus = this.serviceProvider.GetService<IEventBus>();
        }

        public async Task Process(CancellationToken token)
        {
            var queues = new List<string>();
            foreach (var tuple in ReflectionHelpers.TypesThatImplementInterface(t => t == typeof(IHandleCommandsOf<>),
                "NewOrbit.Messaging"))
            {
                var cmdType = tuple.Item1;
                var queue = config.CommandQueue(cmdType);
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
            Log.Logger.Information($"About to start watching CommandQueue {queueName}");
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
                        var cmd = msg.ExtractCommand();
                        Log.Logger.Information($"Found a command of type {cmd.GetType().Name} in the Queue {queueName}");
                        var handlingType = this.registry.GetHandlerFor(cmd);
                        var dispatcher = new CommandDispatcher(cmd, handlingType, this.dependencyFactory,
                            this.sagaDatabase, this.commandBus, this.eventBus);
                        await dispatcher.Dispatch().ConfigureAwait(false);
                        Log.Logger.Information($"Dispatched command of type {cmd.GetType().Name} from queue {queueName}");
                        await queue.DeleteMessageAsync(msg).ConfigureAwait(false);
                        Log.Logger.Information($"Deleted handled command of type {cmd.GetType().Name} from queue {queueName}");
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