using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceUpdatedHandler : IConsumer<IEventInstanceUpdated>
    {
        private readonly ManagementDbContext _managementDbContext;
        readonly ILogger<EventInstanceUpdatedHandler> _logger;

        public EventInstanceUpdatedHandler(
            ManagementDbContext managementDbContext,
            ILogger<EventInstanceUpdatedHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IEventInstanceUpdated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"IEventInstanceUpdated with event series id {context.Message.EventSeriesId}");

                // var series =  _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == context.Message.EventSeriesId);
                //
                // if (series == null)
                // {
                //     _logger.LogInformation($"IEventInstanceUpdated cannot find event series id {context.Message.EventSeriesId}");
                // }
                return Task.CompletedTask;
                //await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}