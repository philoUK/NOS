using Microsoft.WindowsAzure.Storage.Table;

namespace NewOrbit.Messaging.Saga.Azure
{
    public class SagaDataEntity : TableEntity
    {
        public string SagaData { get; set; }
        public string SagaDataType { get; set; }
    }
}
