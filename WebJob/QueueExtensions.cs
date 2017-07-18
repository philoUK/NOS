using System;
using Microsoft.WindowsAzure.Storage.Queue;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event.Azure;
using Newtonsoft.Json;

namespace WebJob
{
    internal static class QueueExtensions
    {
        public static ICommand ExtractCommand(this CloudQueueMessage msg)
        {
            var data = msg.AsString;
            var interim = (QueueWrappedCommandMessage)JsonConvert.DeserializeObject(data, typeof(QueueWrappedCommandMessage));
            var cmdType = Type.GetType(interim.CommandType);
            return (ICommand) JsonConvert.DeserializeObject(interim.CommandJson, cmdType);
        }

        public static IEvent ExtractEvent(this SubscriberQueueWrappedEventMessage msg)
        {
            var data = msg.EventJson;
            var evtType = Type.GetType(msg.EventType);
            return (IEvent) JsonConvert.DeserializeObject(data, evtType);
        }

        public static QueueWrappedEventMessage ExtractEventWrapper(this CloudQueueMessage msg)
        {
            var data = msg.AsString;
            return (QueueWrappedEventMessage)JsonConvert.DeserializeObject(data, typeof(QueueWrappedEventMessage));
        }

        public static SubscriberQueueWrappedEventMessage ExtractEventSubscriptionWrapper(this CloudQueueMessage msg)
        {
            var data = msg.AsString;
            return (SubscriberQueueWrappedEventMessage) JsonConvert.DeserializeObject(data,
                typeof(SubscriberQueueWrappedEventMessage));
        }
    }
}
