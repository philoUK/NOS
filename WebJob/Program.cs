using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;

namespace WebJob
{
    public class Program
    {
        static void Main(string[] args)
        {
            Setup.Initialise(PlatformServices.Default.Application);
            // listen to commands
            var commands = new CommandWatcher(Setup.serviceProvider);
            var events = new EventDistributor(Setup.serviceProvider);
            var subscriptions = new SubscriberDistributor(Setup.serviceProvider);
            var timeouts = new TimeoutWatcher(Setup.serviceProvider);
            
            // listen to events
            // listen to event | subscribers
            // listen to timeouts
            var token = new CancellationToken();
            Task.Run(async () =>
            {
                await Task.WhenAll(commands.Process(token), events.Process(token), subscriptions.Process(token), timeouts.Process(token));
            }, token).Wait(token);
            //var token = new CancellationToken();
            //await Task.WhenAll(commands.Process(token), events.Process(token), timeouts.Process(token));
            Log.Logger.Information("Configuration completed");
            Console.ReadLine();
        }
    }

}