using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceCreatedHandler : IConsumer<IEventInstanceCreated>
    {
        readonly ILogger<EventInstanceCreatedHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public EventInstanceCreatedHandler(
            ILogger<EventInstanceCreatedHandler> logger,
            ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<IEventInstanceCreated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"IEventInstanceCreated with event series id {context.Message.EventSeriesId}");

                var series =
                    _managementDbContext.EventSeries.First(x => x.Id == context.Message.EventSeriesId);

                if (series == null)
                {
                    _logger.LogInformation(
                        $"IEventInstanceUpdated cannot find event series id {context.Message.EventSeriesId}");
                }

                var instanceForSeriesCount =
                    _managementDbContext.EventInstances.Count(x => x.EventSeriesId == series.Id);

                series.EventCount = instanceForSeriesCount;

                await _managementDbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
