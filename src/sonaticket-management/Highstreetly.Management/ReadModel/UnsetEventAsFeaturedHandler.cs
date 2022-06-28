using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class UnsetEventAsFeaturedHandler : IConsumer<IUnsetEventAsFeatured>
    {
        private readonly ManagementDbContext _managementDbContext;
        readonly ILogger<UnsetEventAsFeaturedHandler> _logger;

        public UnsetEventAsFeaturedHandler(ILogger<UnsetEventAsFeaturedHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IUnsetEventAsFeatured> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == context.Message.SourceId);
                series.Featured = false;
                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}