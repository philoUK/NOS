using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga
{
    public class SagaHandlerFactory : IHandlerFactory
    {
        private readonly ISagaDatabase database;
        private readonly IDependencyFactory factory;
        private readonly IClientCommandBus commandBus;
        private readonly IEventBus eventBus;

        public SagaHandlerFactory(ISagaDatabase database, IDependencyFactory factory, IClientCommandBus commandBus, IEventBus eventBus)
        {
            this.database = database;
            this.factory = factory;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
        }

        public async Task<object> Make(Type type, IMessage msg)
        {
            if (type.IsSaga())
            {
                return await this.MakeSaga(type, msg).ConfigureAwait(false);
            }
            return this.factory.Make(type);
        }

        private async Task<object> MakeSaga(Type type, IMessage msg)
        {
            var saga = (ISaga) Activator.CreateInstance(type, new object[] {this.commandBus, this.eventBus});
            var isInDatabase = await this.database.SagaExists(msg.CorrelationId).ConfigureAwait(false);
            if (!isInDatabase)
            {
                saga.Initialise();
                await this.database.Save(saga).ConfigureAwait(false);
            }
            else
            {
                var data = await this.database.LoadSagaData(msg.CorrelationId).ConfigureAwait(false);
                saga.Load(data);
            }
            return saga;
        }
    }
}