using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace BookingApi.Utils
{
    public class NewsletterJob : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 先空實作
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 先空實作
            return Task.CompletedTask;
        }
    }
}
