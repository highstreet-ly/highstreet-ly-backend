using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Infrastructure.Messaging
{
    public class BusClient : IBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IBusControl _ep;
        private readonly IMessageScheduler _scheduler;
        private ILogger<BusClient> _logger;

        public BusClient(
            IBusControl ep,
            IMessageScheduler scheduler,
            IConfiguration configuration,
            ILogger<BusClient> logger)
        {
            _ep = ep;
            _scheduler = scheduler;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Send<T>(
            object values) where T : class, CorrelatedBy<Guid>
        {
            ICommand command;

            try
            {
                command = (ICommand) values;
            }
            catch (Exception)
            {
                _logger.LogWarning($"Attempt to Send {typeof(T).FullName} when Publish should have been used - this is being corrected for you but will need to be fixed in code");
                await Publish<T>(values);
                return;
            }
           

            var delay = command.Delay;
            if (delay != TimeSpan.Zero)
            {
                var scheduledTime = DateTime.UtcNow.Add(delay);
                await _scheduler.SchedulePublish<T>(
                    scheduledTime,
                    values);
                return;
            }

            await _ep.Publish<T>(
                values.ToType<T>(),
                context => context.CorrelationId = ((CorrelatedBy<Guid>) values.ToType<T>()).CorrelationId);
        }

        public async Task Publish<T>(
            object values) where T : class, CorrelatedBy<Guid>
        {
            ISonaticketEvent @event;

            try
            {
                @event = (ISonaticketEvent) values;
            }
            catch (Exception)
            {
                _logger.LogWarning($"Attempt to Publish {typeof(T).FullName} when Send should have been used - this is being corrected for you but will need to be fixed in code");
                await Send<T>(values);
                return;
            }

            var delay = @event.Delay;
            if (delay != TimeSpan.Zero)
            {
                var scheduledTime = DateTime.UtcNow.Add(delay);
                await _scheduler.SchedulePublish<T>(
                    scheduledTime,
                    values);
                return;
            }

            await _ep.Publish<T>(
                values.ToType<T>(),
                context => context.CorrelationId = ((CorrelatedBy<Guid>) values.ToType<T>()).CorrelationId);
        }
    }
}