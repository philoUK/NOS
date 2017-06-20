using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using NewOrbit.Messaging.Timeouts;
using NewOrbit.Messaging.Timeouts.Azure;
using Xunit;

namespace MessagingFacts.Builders
{
    internal class TimeoutDatabaseTestBuilder
    {
        private TimeoutData timeoutData;
        private readonly Mock<ITimeoutDatabaseConfig> config = new Mock<ITimeoutDatabaseConfig>();
        private TableStorageTimeoutDatabase database;

        public TimeoutDatabaseTestBuilder()
        {
            this.config.SetupProperty(c => c.ConnectionString, "UseDevelopmentStorage=true");
            this.config.SetupProperty(c => c.TableName, "testtimeoutdatabase");
        }

        public TimeoutDatabaseTestBuilder GivenATimeoutRequest()
        {
            this.timeoutData = new TimeoutData
            {
                TargetId = Guid.NewGuid().ToString(),
                TargetMethod = "MethodName",
                TargetType = "SomeTypeInfo",
                Timeout = DateTime.UtcNow.AddMinutes(3)
            };
            return this;
        }

        public TimeoutDatabaseTestBuilder Save()
        {
            this.database = new TableStorageTimeoutDatabase(this.config.Object);
            this.database.Save(this.timeoutData);
            return this;
        }

        public void VerifyMessageStoredInTheCorrectQueue()
        {
            var cloudConfig = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var cloudTableClient = cloudConfig.CreateCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("testtimeoutdatabase");
            var exists = cloudTable.ExistsAsync().Result;
            Assert.True(exists);
            // check we can get a message of id x | y
            var op = TableOperation.Retrieve(this.timeoutData.TargetId, this.timeoutData.TargetMethod);
            var result = cloudTable.ExecuteAsync(op).Result;
            Assert.NotNull(result.Result);
            cloudTable.DeleteAsync().Wait();
        }

        public void VerifyMessageFormattedProperly()
        {
            Assert.True(this.timeoutData.TargetId == this.database.StoredData.PartitionKey);
            Assert.True(this.database.StoredData.RowKey == this.timeoutData.TargetMethod);
            Assert.True(this.timeoutData.TargetMethod == this.database.StoredData.OwnerMethod);
            Assert.True(this.timeoutData.TargetType == this.database.StoredData.OwnerType);
            Assert.True(this.timeoutData.Timeout == this.database.StoredData.Timeout);
        }


        public TimeoutDatabaseTestBuilder Delete()
        {
            this.database.Delete(this.timeoutData.TargetId, this.timeoutData.TargetMethod);
            return this;
        }

        public void VerifyMessageNotFound()
        {
            var cloudConfig = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var cloudTableClient = cloudConfig.CreateCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("testtimeoutdatabase");
            var exists = cloudTable.ExistsAsync().Result;
            Assert.True(exists);
            // check we can get a message of id x | y
            var op = TableOperation.Retrieve(this.timeoutData.TargetId, this.timeoutData.TargetMethod);
            var result = cloudTable.ExecuteAsync(op).Result;
            Assert.Null(result.Result);
            cloudTable.DeleteAsync().Wait();
        }
    }
}
