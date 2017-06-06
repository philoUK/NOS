using System;
using System.Threading.Tasks;
using NewOrbit.Messaging.Monitoring.Events;
using NewOrbit.Messaging.Registrars;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Event.Azure
{
    public class ReceivingEventBus : IPublishEventsOf<EventDispatched>, IReceivingEventBus
    {
        private readonly IHandlerFactory handlerFactory;
        private readonly IEventBus bus;

        public ReceivingEventBus(IHandlerFactory handlerFactory, IEventBus bus)
        {
            this.handlerFactory = handlerFactory;
            this.bus = bus;
        }

        public async Task Dispatch(QueueWrappedEventMessage message)
        {
            IEvent @event = this.UnpackEvent(message);
            var subscriber = this.GetSubscriber(message);
            this.Handle(@event, subscriber);
            await this.PublishSuccess(@event, subscriber).ConfigureAwait(false);
        }

        private IEvent UnpackEvent(QueueWrappedEventMessage message)
        {
            var json = message.EventJson;
            var type = Type.GetType(message.EventType);
            try
            {
                return (IEvent) json.FromJson(type);
            }
            catch(Exception ex)
            {
                throw new MessageUnpackingException($"Error deserialising the IEvent from a QueueWrappedEventMessage", ex);
            }
        }

        private object GetSubscriber(QueueWrappedEventMessage message)
        {
            var type = Type.GetType(message.SubscribingType);
            try
            {
                return this.handlerFactory.Make(type);
            }
            catch (Exception ex)
            {
                throw new MessageUnpackingException($"Error determining the subscriber for event of type {message.EventType} with an Id of {message.EventId}", ex);
            }
        }

        private void Handle(IEvent @event, object subscriber)
        {
            var interfaceType = subscriber.GetGenericInterface(i => i == typeof(ISubscribeToEventsOf<>), @event.GetType());
            if (interfaceType != null)
            {
                var method = interfaceType.GetMethod("HandleEvent");
                method.Invoke(subscriber, new object[] { @event });
            }
        }

        private async Task PublishSuccess(IEvent @event, object subscriber)
        {
            var msg = new EventDispatched
            {
                Date = DateTime.UtcNow,
                EventId = @event.Id,
                EventTypeName = @event.GetType().AssemblyQualifiedName,
                SubscriberTypeName = subscriber.GetType().AssemblyQualifiedName
            };
            await this.bus.Publish(this, msg).ConfigureAwait(false);
        }
    }
}
