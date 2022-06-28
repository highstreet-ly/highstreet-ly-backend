using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.QuartzIntegration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Highstreetly.Scheduler
{
    public class MassTransitConsoleHostedService : IHostedService
    {
        readonly IBusControl _bus;
        readonly ILogger _logger;
        readonly IScheduler _scheduler;

        public MassTransitConsoleHostedService(
            IBusControl bus, 
            ILoggerFactory loggerFactory, 
            IScheduler scheduler)
        {
            _bus = bus;

            _logger = loggerFactory.CreateLogger<MassTransitConsoleHostedService>();

            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus");
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

            _scheduler.JobFactory = new MassTransitJobFactory(_bus, null);
            try
            {
                _logger.LogInformation("Starting scheduler");
                await _scheduler.Start(cancellationToken);
            }
            catch (Exception)
            {
                await _scheduler.Shutdown(cancellationToken);

                throw;
            }

            _logger.LogInformation("Started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Standby(cancellationToken);

            _logger.LogInformation("Stopping");
            await _bus.StopAsync(cancellationToken);

            await _scheduler.Shutdown(cancellationToken);

            _logger.LogInformation("Stopped");
        }
    }
}