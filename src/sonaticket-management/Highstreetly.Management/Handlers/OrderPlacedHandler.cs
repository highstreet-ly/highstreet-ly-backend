using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class OrderPlacedHandler : OrderEventHandlerBase<OrderPlacedHandler>, IConsumer<IOrderPlaced>
    {
        private readonly ILogger<OrderPlacedHandler> _logger;

        public OrderPlacedHandler( ILogger<OrderPlacedHandler> logger, ManagementDbContext managementDbContext) : base(logger, managementDbContext)
        {
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IOrderPlaced> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                var eventMessage = @event.Message;
                try
                {
                    var newOrder = new Order
                    {
                        CreatedOn = DateTime.UtcNow,
                        EventInstanceId = eventMessage.EventInstanceId,
                        Id = eventMessage.SourceId,
                        OwnerId = eventMessage.OwnerId,
                        HumanReadableId = eventMessage.HumanReadableId,
                        IsLocalDelivery = eventMessage.IsLocalDelivery,
                        IsNationalDelivery = eventMessage.IsNationalDelivery,
                        IsClickAndCollect = eventMessage.IsClickAndCollect,
                        IsToTable = eventMessage.IsToTable,
                        TableInfo = eventMessage.TableInfo,
                    };

                    await ManagementDbContext.Orders.AddAsync(newOrder);

                    //another check
                    if (ManagementDbContext.Orders.FirstOrDefault(x => x.Id == @event.Message.SourceId) == null)
                    {
                        await ManagementDbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Couldn't run IOrderPlaced for {eventMessage.SourceId}", ex);
                    throw;
                }
            }
        }
    }
}