using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Shared;
using Web.Logging;

namespace Web.Config
{
    public static class MessageExtensions
    {
        public static void AddNewOrbitMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IAzureStorageQueueConfig, AzureStorageQueueConfig>();
            services.AddTransient<IDeferredEventMechanism, AzureStorageQueueEventMechanism>();
            services.AddSingleton<IEventSubscriberRegistry, EventRegistry>();
            services.AddTransient<ILogEventBusMessages, LogEventBusMessages>();
            services.AddSingleton<IEventPublisherRegistry, EventRegistry>();
            services.AddTransient<IEventBus, DeferredEventBus>();
            services.AddSingleton<ICommandHandlerRegistry, CommandHandlerRegistry>();
            services.AddTransient<IDeferredCommandMechanism, AzureStorageQueueCommandMechanism>();
            services.AddTransient<IClientCommandBus, DeferredClientCommandBus>();
        }
    }
}
