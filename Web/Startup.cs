using System;
using System.Spatial;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Web.Config;

namespace Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            this.ConfigureLogging(env.EnvironmentName.ToLower());
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddNewOrbitMessaging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
         
            Log.Verbose("Application Startup complete");
        }

        private void ConfigureLogging(string environmentName)
        {
            var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var loggerConfiguration =
                new LoggerConfiguration().WriteTo.Trace()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", environmentName)
                    .Enrich.WithProperty("Application", "SagaSample")
                    .Enrich.WithProperty("Context", "Web");
            loggerConfiguration.MinimumLevel.ControlledBy(levelSwitch);
            loggerConfiguration.WriteTo.Seq(this.Configuration["Data:Seq:Url"],
                apiKey: this.Configuration["Data:Seq:ApiKey"]);
            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
