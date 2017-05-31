using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Command.Azure
{
    public class ReceivingCommandBus
    {
        private ICommandHandlerRegistry registry;
        private IEventBus eventBus;

        public ReceivingCommandBus(ICommandHandlerRegistry registry, IEventBus eventBus)
        {
            this.registry = registry;
            this.eventBus = eventBus;
        }

        public async Task Submit(QueueWrappedMessage buildMessage)
        {
            var cmd = await this.ExtractCommand(buildMessage).ConfigureAwait(false);
            var handler = this.GetHandler(cmd);
            this.DispatchToHandler(cmd, handler);
        }

        private async Task<ICommand> ExtractCommand(QueueWrappedMessage msg)
        {
            try
            {
                var cmdType = Type.GetType(msg.CommandType);
                return (ICommand) msg.CommandJson.FromJson(cmdType);
            }
            catch (Exception ex)
            {
                var err = $"Failed to Extract a command of type {msg.CommandType} for Command with Id {msg.CommandId}";
                await this.PublishExtractError(err, msg).ConfigureAwait(false);
                throw new MessageUnpackingException(err, ex);
            }
        }

        private async Task PublishExtractError(string errMessage, QueueWrappedMessage msg)
        {
            var @event = new CommandCouldNotBeReadEvent
            {
                CommandId = msg.CommandId,
                CommandType = msg.CommandType,
                ErrorMessage = errMessage
            };
            await this.eventBus.Publish(@event).ConfigureAwait(false);
        }

        private object GetHandler(ICommand cmd)
        {
            var handlingType = this.registry.GetHandlerFor(cmd);
            return Activator.CreateInstance(handlingType);
        }

        private void DispatchToHandler(ICommand cmd, object handler)
        {
            var interfaceType = handler.GetType()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleCommandsOf<>) &&
                            i.GetGenericArguments()[0] == cmd.GetType());
            if (interfaceType != null)
            {
                var method = interfaceType.GetMethod("Handle");
                method.Invoke(handler, new object[] {cmd});
            }
        }
    }
}