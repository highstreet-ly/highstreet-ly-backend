using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Management.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class OrderExpiredHandler : OrderEventHandlerBase<OrderExpiredHandler>, IConsumer<IOrderExpired>
    {
        private readonly ILogger<OrderExpiredHandler> _logger;

        public OrderExpiredHandler(
            ManagementDbContext managementDbContext,
            ILogger<OrderExpiredHandler> logger)
            : base(logger, managementDbContext)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderExpired> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                try
                {
                    var order = ManagementDbContext.Orders.First(x => x.Id == @event.Message.SourceId);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Expired;
                        order.ExpiredDateTime = DateTime.UtcNow;

                        await ManagementDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        Logger.LogCritical(
                            $"Order with ID {@event.Message.SourceId} was not found in {nameof(OrderExpiredHandler)} - cannot expire order");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Couldn't run IOrderExpired", ex);
                    throw;
                }
            }
        }
    }
}