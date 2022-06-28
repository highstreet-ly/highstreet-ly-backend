using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class UnPublishEventInstanceHandler : IConsumer<IUnPublishEventInstance>
    {
        private readonly ILogger<UnPublishEventInstanceHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public UnPublishEventInstanceHandler(
            ManagementDbContext managementDbContext,
            ILogger<UnPublishEventInstanceHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IUnPublishEventInstance> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"Running Consume {nameof(IUnPublishEventInstance)}");

                var eventInstance = _managementDbContext
                    .EventInstances
                    .FirstOrDefault(x => x.Slug == context.Message.Slug);

                if (eventInstance == null)
                {
                    throw new Exception();
                }

                eventInstance.IsPublished = false;

                await _managementDbContext.SaveChangesAsync();

                await context.Publish<IEventInstanceUnpublished>(new
                    { SourceId = eventInstance.Id, context.Message.CorrelationId });
            }
        }
    }
}
