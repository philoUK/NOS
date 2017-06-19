using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga.Azure
{
    public class TableStorageSagaDatabase : ISagaDatabase
    {
        private readonly ISagaDatabaseConfig config;
        private CloudTable table;

        public TableStorageSagaDatabase(ISagaDatabaseConfig config)
        {
            this.config = config;
        }

        public async Task<bool> SagaExists(string id)
        {
            await this.CreateTable().ConfigureAwait(false);
            var retrieveOperation = TableOperation.Retrieve<SagaDataEntity>(id, "");
            var result = await this.table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);
            return result.Result != null;
        }

        public async Task Save(ISaga saga)
        {
            await this.CreateTable().ConfigureAwait(false);
            if (await this.SagaExists(saga.SagaId).ConfigureAwait(false))
            {
                await this.UpdateSaga(saga).ConfigureAwait(false);
            }
            else
            {
                await this.CreateSaga(saga).ConfigureAwait(false);
            }
        }

        private async Task UpdateSaga(ISaga saga)
        {
            var retrieveOp = TableOperation.Retrieve<SagaDataEntity>(saga.SagaId, "");
            var result = await this.table.ExecuteAsync(retrieveOp).ConfigureAwait(false);
            var entity = (SagaDataEntity) result.Result;
            entity.SagaData = saga.SagaData.ToJson();
            entity.SagaDataType = saga.SagaData.GetType().AssemblyQualifiedName;
            var updateOp = TableOperation.Replace(entity);
            await this.table.ExecuteAsync(updateOp).ConfigureAwait(false);
        }

        private async Task CreateSaga(ISaga saga)
        {
            var entity = new SagaDataEntity
            {
                PartitionKey = saga.SagaId,
                RowKey = "",
                SagaData = saga.SagaData.ToJson(),
                SagaDataType = saga.SagaData.GetType().AssemblyQualifiedName
            };
            var insertOp = TableOperation.Insert(entity);
            await this.table.ExecuteAsync(insertOp).ConfigureAwait(false);
        }

        public async Task<ISagaData> LoadSagaData(string sagaId)
        {
            await this.CreateTable().ConfigureAwait(false);
            if (await this.SagaExists(sagaId).ConfigureAwait(false))
            {
                var op = TableOperation.Retrieve<SagaDataEntity>(sagaId, "");
                var result = await this.table.ExecuteAsync(op).ConfigureAwait(false);
                var entity = (SagaDataEntity) result.Result;
                var type = Type.GetType(entity.SagaDataType);
                return (ISagaData) entity.SagaData.FromJson(type);
            }
            return null;
        }

        private async Task CreateTable()
        {
            if (this.table != null)
            {
                return;
            }
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            this.table = client.GetTableReference(this.config.TableName);
            await this.table.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public async Task DeleteSagaData(string sagaId)
        {
            await this.CreateTable().ConfigureAwait(false);
            if (await this.SagaExists(sagaId).ConfigureAwait(false))
            {
                var op = TableOperation.Retrieve<SagaDataEntity>(sagaId, "");
                var entity = await this.table.ExecuteAsync(op).ConfigureAwait(false);
                var deleteOp = TableOperation.Delete((ITableEntity) entity.Result);
                await this.table.ExecuteAsync(deleteOp).ConfigureAwait(false);
            }
        }
    }
}
