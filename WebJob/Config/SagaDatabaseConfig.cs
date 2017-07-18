using Microsoft.Extensions.Configuration;
using NewOrbit.Messaging.Saga.Azure;

namespace WebJob.Config
{
    class SagaDatabaseConfig : ISagaDatabaseConfig
    {
        public SagaDatabaseConfig(IConfigurationRoot root)
        {
            var section = root.GetSection("saga");
            this.ConnectionString = section["connectionString"];
            this.TableName = section["table"];
        }
        public string ConnectionString { get; }
        public string TableName { get; }
    }
}
