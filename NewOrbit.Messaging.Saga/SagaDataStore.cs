using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga
{
    public class SagaDataStore : IHandlerFactory
    {
        private readonly ISagaDatabase database;
        private readonly IDependencyFactory factory;

        public SagaDataStore(ISagaDatabase database, IDependencyFactory factory)
        {
            this.database = database;
            this.factory = factory;
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
            var saga = (ISaga) this.factory.Make(type);
            var isInDatabase = await this.database.SagaExists(msg).ConfigureAwait(false);
            if (!isInDatabase)
            {
                saga.Initialise();
                await this.database.Save(saga).ConfigureAwait(false);
            }
            else
            {
                var data = await this.database.LoadSagaData(saga).ConfigureAwait(false);
                saga.Load(data);
            }
            return saga;
        }
    }
}