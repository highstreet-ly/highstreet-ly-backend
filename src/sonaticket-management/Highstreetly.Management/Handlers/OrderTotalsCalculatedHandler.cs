using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class OrderTotalsCalculatedHandler
        : OrderEventHandlerBase<OrderTotalsCalculatedHandler>, IConsumer<IOrderTotalsCalculated>
    {
        public OrderTotalsCalculatedHandler(
            ILogger<OrderTotalsCalculatedHandler> logger,
            ManagementDbContext managementDbContext)
            : base(logger, managementDbContext) { }

        public async Task Consume(
            ConsumeContext<IOrderTotalsCalculated> @event)
        {
            using (Logger.BeginScope(@event))
            {
                if (!await ProcessOrder(order => order.Id == @event.Message.SourceId, order =>
                {
                    order.TotalAmount = @event.Message.Total;
                    order.PaymentPlatformFees = @event.Message.PaymentPlatformFees;
                    order.PlatformFees = @event.Message.PlatformFees;
                    return Task.CompletedTask;
                }))
                    Logger.LogError("Failed to locate the order with id {0} to apply calculated totals",
                        @event.Message.SourceId);
            }
        }
    }
}