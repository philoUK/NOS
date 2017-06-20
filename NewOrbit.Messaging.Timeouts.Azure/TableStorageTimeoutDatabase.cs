using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NewOrbit.Messaging.Timeouts.Azure
{
    public class TableStorageTimeoutDatabase : ITimeoutDatabase
    {
        private ITimeoutDatabaseConfig config;
        private CloudTable table;
        private TimeoutEntity entity;

        public TableStorageTimeoutDatabase(ITimeoutDatabaseConfig config)
        {
            this.config = config;
            this.CreateTable();
        }

        public TimeoutEntity StoredData => this.entity;

        private void CreateTable()
        {
            var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            this.table = client.GetTableReference(this.config.TableName);
            this.table.CreateIfNotExistsAsync().Wait();
        }

        public void Save(TimeoutData timeoutData)
        {
            this.entity = new TimeoutEntity
            {
                PartitionKey = timeoutData.TargetId,
                RowKey = timeoutData.TargetMethod,
                OwnerMethod = timeoutData.TargetMethod,
                Timeout = timeoutData.Timeout,
                OwnerType = timeoutData.TargetType
            };
            var insertOp = TableOperation.Insert(this.entity);
            this.table.ExecuteAsync(insertOp).Wait();
        }

        public void Delete(string targetId, string targetMethod)
        {
            var fetchOp = TableOperation.Retrieve<TimeoutEntity>(targetId, targetMethod);
            var results = this.table.ExecuteAsync(fetchOp).Result;
            if (results.Result != null)
            {
                var entity = results.Result as TimeoutEntity;
                var deleteOp = TableOperation.Delete(entity);
                this.table.ExecuteAsync(deleteOp).Wait();
            }
        }

        public IEnumerable<TimeoutData> GetExpiredTimeoutsSince(DateTime dtm)
        {
            var results = new List<TimeoutEntity>();
            TableQuery<TimeoutEntity> query = new TableQuery<TimeoutEntity>()
                .Where(TableQuery.GenerateFilterConditionForDate("Timeout", QueryComparisons.LessThan,
                    DateTimeOffset.UtcNow));
            TableContinuationToken continuationToken = null;
            do
            {
                var items = table.ExecuteQuerySegmentedAsync(query, continuationToken).Result;
                foreach (var item in items)
                {
                    results.Add(item);
                }
                continuationToken = items.ContinuationToken;
            } while (continuationToken != null);
            return results.Select(ToData);
        }

        private TimeoutData ToData(TimeoutEntity arg)
        {
            return new TimeoutData
            {
                TargetId = arg.PartitionKey,
                TargetMethod = arg.OwnerMethod,
                TargetType = arg.OwnerType,
                Timeout = arg.Timeout
            };
        }
    }
}
