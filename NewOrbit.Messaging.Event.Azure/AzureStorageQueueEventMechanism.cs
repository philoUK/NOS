using System;
using System.Threading.Tasks;

namespace NewOrbit.Messaging.Event.Azure
{
    public class AzureStorageQueueEventMechanism : IDeferredEventMechanism
    {
        public Task Defer(IEvent @event, Type subscribingType)
        {
            throw new NotImplementedException();
        }
    }
}
