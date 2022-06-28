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
    public class PublishEventInstanceHandler : IConsumer<IPublishEventInstance>
    {
        private readonly ILogger<PublishEventInstanceHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public PublishEventInstanceHandler(ManagementDbContext managementDbContext,
            ILogger<PublishEventInstanceHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IPublishEventInstance> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation($"Running Consume {nameof(IPublishEventInstance)}");

                var eventInstance = _managementDbContext
                    .EventInstances
                    .FirstOrDefault(x => x.Slug == context.Message.Slug);

                if (eventInstance == null)
                {
                    throw new Exception();
                }

                eventInstance.IsPublished = context.Message.Published;

                
                await _managementDbContext.SaveChangesAsync();

                if (context.Message.Published)
                {
                    await context.Publish<IEventInstancePublished>(new
                    {
                        SourceId = eventInstance.Id,
                        context.Message.CorrelationId
                    });
                }
                else
                {
                    await context.Publish<IEventInstanceUnpublished>(new
                        {SourceId = eventInstance.Id, context.Message.CorrelationId});
                }
            }
        }
    }
}