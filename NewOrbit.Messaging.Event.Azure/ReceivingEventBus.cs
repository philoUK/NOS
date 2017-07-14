using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Event.Azure
{
    public class ReceivingEventBus : IPublishEventsOf<EventDispatched>, IReceivingEventBus
    {
        private readonly IEventBus bus;
        private readonly IEventSubscriberRegistry registry;
        private readonly IDeferredEventMechanism mechanism;

        public ReceivingEventBus(IEventBus bus, IEventSubscriberRegistry registry, IDeferredEventMechanism mechanism)
        {
            this.bus = bus;
            this.registry = registry;
            this.mechanism = mechanism;
        }

        public async Task Dispatch(QueueWrappedEventMessage message)
        {
            IEvent @event = this.UnpackEvent(message.EventJson, message.EventType);
            foreach (var subscriber in this.registry.GetSubscribers(@event))
            {
                // queue up the actual directed message
                await this.mechanism.DeferToSubscriber(@event, subscriber).ConfigureAwait(false);
                await this.PublishSuccess(@event, subscriber).ConfigureAwait(false);
            }
        }

        private IEvent UnpackEvent(string eventJson, string eventType)
        {
            var type = Type.GetType(eventType);
            try
            {
                return (IEvent) eventJson.FromJson(type);
            }
            catch(Exception ex)
            {
                throw new MessageUnpackingException($"Error deserialising the IEvent from a QueueWrappedEventMessage", ex);
            }
        }

        private async Task PublishSuccess(IEvent @event, Type subscriber)
        {
            var msg = new EventDispatched
            {
                Date = DateTime.UtcNow,
                EventId = @event.Id,
                EventTypeName = @event.GetType().AssemblyQualifiedName,
                SubscriberTypeName = subscriber.AssemblyQualifiedName
            };
            await this.bus.Publish(this, msg).ConfigureAwait(false);
        }

        //public void Dispatch(SubscriberQueueWrappedEventMessage message)
        //{
        //    IEvent @event = this.UnpackEvent(message.EventJson, message.EventType);
        //    var subscriberType = Type.GetType(message.SubscriberType);
        //    var subscriber = this.dependencyFactory.Make(subscriberType);
        //    this.Handle(@event, subscriber);
        //}

        //private void Handle(IEvent @event, object subscriber)
        //{
        //    var interfaceType = subscriber.GetGenericInterface(typeof(ISubscribeToEventsOf<>), @event.GetType());
        //    if (interfaceType != null)
        //    {
        //        var method = interfaceType.GetMethod("HandleEvent");
        //        method.Invoke(subscriber, new object[] { @event });
        //    }
        //}
    }
}
