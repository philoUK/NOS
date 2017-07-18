using Microsoft.Extensions.DependencyInjection;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Shared;

namespace Web.Config
{
    public static class MessageExtensions
    {
        public static void AddNewOrbitMessaging(this IServiceCollection services)
        {
            services.AddTransient<IDeferredEventMechanism, AzureStorageQueueEventMechanism>();
            services.AddSingleton<IEventPublisherRegistry, EventRegistry>();
            services.AddSingleton<IAzureStorageQueueConfig, AzureStorageQueueConfig>();
            services.AddTransient<ICommandHandlerRegistry, CommandHandlerRegistry>();
            services.AddTransient<IEventBus, DeferredEventBus>();
            services.AddTransient<IDeferredCommandMechanism, AzureStorageQueueCommandMechanism>();
            services.AddTransient<IClientCommandBus, DeferredClientCommandBus>();
        }
    }
}
