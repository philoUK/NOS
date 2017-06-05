using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Command.Azure
{
    public class AzureStorageQueueCommandMechanism : IDeferredCommandMechanism
    {
        private readonly ICommandHandlerRegistry registry;
        private readonly IAzureStorageQueueConfig config;

        public AzureStorageQueueCommandMechanism(ICommandHandlerRegistry registry, IAzureStorageQueueConfig config)
        {
            this.registry = registry;
            this.config = config;
        }

        public async Task Defer(ICommand command)
        {
            await this.QueueMessage(this.ConstructMessage(command)).ConfigureAwait(false);
        }

        private QueueWrappedCommandMessage ConstructMessage(ICommand command)
        {
            var msg = new QueueWrappedCommandMessage
            {
                CommandId = command.Id,
                Date = DateTime.UtcNow,
                CommandType = command.GetType().AssemblyQualifiedName,
                CommandJson = command.ToJson()
            };
            return msg;
        }

        protected virtual async Task QueueMessage(QueueWrappedCommandMessage msg)
        {
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var cmdType = Type.GetType(msg.CommandType);
            var queue = client.GetQueueReference(this.config.CommandQueue(cmdType));
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
            await queue.AddMessageAsync(new CloudQueueMessage(msg.ToJson())).ConfigureAwait(false);
        }
    }
}
