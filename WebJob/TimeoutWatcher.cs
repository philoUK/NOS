using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebJob
{
    class TimeoutWatcher
    {
        private IServiceProvider serviceProvider;

        public TimeoutWatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task Process(CancellationToken token)
        {
            return Task.Delay(TimeSpan.FromMinutes(1), token);
        }
    }
}