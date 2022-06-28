using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceUnpublishedHandler : IConsumer<IEventInstanceUnpublished>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly ILogger<EventInstanceUnpublishedHandler> _logger;

        public EventInstanceUnpublishedHandler(
            ManagementDbContext managementDbContext,
            ILogger<EventInstanceUnpublishedHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IEventInstanceUnpublished> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"Running Consume {nameof(IEventInstanceUnpublished)}");

                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == context.Message.SourceId);
                var allInSeries =
                    _managementDbContext.EventInstances.Where(x => x.EventSeriesId == eventInstance.EventSeriesId);
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == eventInstance.EventSeriesId);

                if (allInSeries.Any())
                {
                    var allUnPublished = allInSeries.Any(x => x.IsPublished == false);
                    series.IsPublished = allUnPublished;
                }
                else
                {
                    // this is the only event in the series so we are definitely not publsihed anymore
                    series.IsPublished = false;
                }

                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}