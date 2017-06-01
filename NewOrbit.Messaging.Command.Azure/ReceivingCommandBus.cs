using System;
using System.Linq;
using System.Reflection;
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
        private IHandlerFactory handlerFactory;

        public ReceivingCommandBus(ICommandHandlerRegistry registry, IEventBus eventBus, IHandlerFactory handlerFactory)
        {
            this.registry = registry;
            this.eventBus = eventBus;
            this.handlerFactory = handlerFactory;
        }

        public async Task Submit(QueueWrappedMessage buildMessage)
        {
            var cmd = await this.ExtractCommand(buildMessage).ConfigureAwait(false);
            var handler = await this.GetHandler(cmd).ConfigureAwait(false);
            this.DispatchToHandler(cmd, handler);
            await this.PublishSuccess(cmd).ConfigureAwait(false);
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

        private async Task<object> GetHandler(ICommand cmd)
        {
            try
            {
                var handlingType = this.registry.GetHandlerFor(cmd);
                return this.handlerFactory.Make(handlingType);
            }
            catch (NoCommandHandlerDefinedException)
            {
                var @event = new CommandDidNotDefineAHandlerEvent
                {
                    CommandId = cmd.Id,
                    CommandType = cmd.GetType().AssemblyQualifiedName
                };
                await this.eventBus.Publish(@event).ConfigureAwait(false);
                throw;
            }
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

        private async Task PublishSuccess(ICommand cmd)
        {
            var @event = new CommandWasDispatchedEvent
            {
                CommandId = cmd.Id,
                CommandType = cmd.GetType().AssemblyQualifiedName
            };
            await this.eventBus.Publish(@event).ConfigureAwait(false);
        }
    }
}