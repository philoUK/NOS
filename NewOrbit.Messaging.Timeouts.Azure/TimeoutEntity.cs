using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace NewOrbit.Messaging.Timeouts.Azure
{
    public class TimeoutEntity : TableEntity 
    {
        public string OwnerMethod { get; set; }
        public DateTime Timeout { get; set; }
        public string OwnerType { get; set; }
    }
}
