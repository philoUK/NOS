using System.Collections.Generic;
using System.Linq;

namespace NewOrbit.Messaging
{
    public class CommandBus
    {
        private readonly IGetCommandHandler getCommandHandler;
        private readonly ICommandBusLogger logger;

        public CommandBus(IGetCommandHandler getCommandHandler, ICommandBusLogger logger)
        {
            this.getCommandHandler = getCommandHandler;
            this.logger = logger;
        }

        public void Submit(ICommand cmd)
        {
            var handlers = this.getCommandHandler.GetCommands(cmd).ToList();
            EnsureThereIsOnlyOneHandler(cmd, handlers);
            handlers.SingleOrDefault().InvokeCommand(cmd);
        }

        private void EnsureThereIsOnlyOneHandler(ICommand cmd, List<IInvokeCommands> handlers)
        {
            if (handlers.Count > 1)
            {
                this.logger.LogMultipleCommandHandlerException(cmd);
                throw new MultipleCommandHandlersDefinedException(cmd);
            }
            if (handlers.Count == 0)
            {
                this.logger.LogNoCommandHandlerException(cmd);
                throw new NoCommandHandlersDefinedException(cmd);
            }
        }
    }
}
