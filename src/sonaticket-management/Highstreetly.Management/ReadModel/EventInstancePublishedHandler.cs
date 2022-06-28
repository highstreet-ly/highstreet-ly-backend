using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstancePublishedHandler : IConsumer<IEventInstancePublished>
    {
        private readonly ManagementDbContext _managementDbContext;
        readonly ILogger<EventInstancePublishedHandler> _logger;

        public EventInstancePublishedHandler(
            ManagementDbContext managementDbContext,
            ILogger<EventInstancePublishedHandler> logger)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<IEventInstancePublished> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"Running Consume {nameof(IEventInstancePublished)}");
                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == context.Message.SourceId);
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == eventInstance.EventSeriesId);
                series.IsPublished = true;
                series.WasEverPublished = true;
                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}