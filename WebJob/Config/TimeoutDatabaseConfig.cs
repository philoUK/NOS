using Microsoft.Extensions.Configuration;
using NewOrbit.Messaging.Timeouts.Azure;

namespace WebJob.Config
{
    internal class TimeoutDatabaseConfig : ITimeoutDatabaseConfig
    {
        public TimeoutDatabaseConfig(IConfigurationRoot root)
        {
            var section = root.GetSection("timeout");
            this.ConnectionString = section["connectionString"];
            this.TableName = section["tableName"];
        }

        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}
