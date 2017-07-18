using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Command;
using NewOrbit.Messaging.Command.Azure;
using NewOrbit.Messaging.Event;
using NewOrbit.Messaging.Event.Azure;
using NewOrbit.Messaging.Saga;
using NewOrbit.Messaging.Saga.Azure;
using NewOrbit.Messaging.Shared;
using NewOrbit.Messaging.Timeouts;
using NewOrbit.Messaging.Timeouts.Azure;
using Serilog;
using WebJob.Config;

namespace WebJob
{
    static class Setup
    {
        private static IConfigurationRoot configuration;
        private static IServiceCollection services;
        internal static IServiceProvider serviceProvider;

        public static void Initialise(ApplicationEnvironment appEnv)
        {
            SetupConfiguration(appEnv);
            SetupLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            SetupDependencyInjection();
        }

        private static void SetupConfiguration(ApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($@"{appEnv.ApplicationBasePath}\config\appsettings.json")
                .AddJsonFile($@"{appEnv.ApplicationBasePath}\config\appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables();
            configuration = builder.Build();
        }

        private static void SetupLogging(string environmentName)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Trace()
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Context", "WebJob")
                .Enrich.WithProperty("Application", "Sample Domain");

            loggerConfiguration.WriteTo.Seq(
                configuration["Data:Seq:Url"], apiKey: configuration["Data:Seq:ApiKey"]);

            Log.Logger = loggerConfiguration
                .CreateLogger();
        }

        private static void SetupDependencyInjection()
        {
            services = new ServiceCollection();
            AddBuiltInServices();
            AddMessagingInfrastructure();
        }

        private static void AddBuiltInServices()
        {
            services.AddSingleton<IConfigurationRoot>(configuration);
        }

        private static void AddMessagingInfrastructure()
        {
            services.AddTransient<IDeferredEventMechanism, AzureStorageQueueEventMechanism>();
            services.AddSingleton<IEventSubscriberRegistry, EventRegistry>();
            services.AddTransient<ITimeoutDatabaseConfig, TimeoutDatabaseConfig>();
            services.AddTransient<ITimeoutDatabase, TableStorageTimeoutDatabase>();
            services.AddTransient<ISagaDatabaseConfig, SagaDatabaseConfig>();
            services.AddTransient<ISagaDatabase, TableStorageSagaDatabase>();
            services.AddSingleton<IDependencyFactory>(
                new DIDependencyFactory(new Lazy<IServiceProvider>(() => Setup.serviceProvider)));
            // command bus and its chain [includes event bus and its chain]
            services.AddTransient<IDeferredEventMechanism, AzureStorageQueueEventMechanism>();
            services.AddSingleton<IEventPublisherRegistry, EventRegistry>();
            services.AddSingleton<IAzureStorageQueueConfig, AzureStorageQueueConfig>();
            services.AddSingleton<ICommandHandlerRegistry, CommandHandlerRegistry>();
            services.AddTransient<IEventBus, DeferredEventBus>();
            services.AddTransient<IDeferredCommandMechanism, AzureStorageQueueCommandMechanism>();
            services.AddTransient<IClientCommandBus, DeferredClientCommandBus>();
            serviceProvider = services.BuildServiceProvider();
        }
    }

}
