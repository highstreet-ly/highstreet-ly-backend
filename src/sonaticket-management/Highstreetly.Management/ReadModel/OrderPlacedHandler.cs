using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class OrderPlacedHandler : IConsumer<IOrderPlaced>
    {
        private readonly ILogger<OrderPlacedHandler> _logger;


        public OrderPlacedHandler(
            ILogger<OrderPlacedHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IOrderPlaced> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation(
                    $"Consuming IOrderPlaced in OrderPlacedHandler for {context.Message.SourceId}");

                return Task.CompletedTask;
            }
        }
    }
}