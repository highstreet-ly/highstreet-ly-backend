using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.StripeIntegration;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeCreatedHandler : IConsumer<ITicketTypeCreated>
    {
        private IStripeProductService _stripeProductService;
        private readonly ILogger<TicketTypeCreatedHandler> _logger;

        public TicketTypeCreatedHandler(
            IStripeProductService stripeProductService,
            ILogger<TicketTypeCreatedHandler> logger)
        {
            _stripeProductService = stripeProductService;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ITicketTypeCreated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                // await _stripeProductService.EnsureProductExistsOnStripe(
                //     context.Message.Name,
                //     context.Message.SourceId,
                //     (long) context.Message.Price);

                await context.Publish<ITicketTypePublished>(new TicketTypePublished
                {
                    EventInstanceId = context.Message.EventInstanceId,
                    SourceId = context.Message.SourceId,
                    CorrelationId = context.Message.CorrelationId,
                });
            }
        }
    }


}