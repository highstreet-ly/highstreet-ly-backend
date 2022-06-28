using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class OrderConfirmedHandler : OrderEventHandlerBase<OrderConfirmedHandler>, IConsumer<IOrderConfirmed>
    {
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly ILogger<OrderConfirmedHandler> _logger;
        private readonly INotificationSenderService _notificationSenderService;

        public OrderConfirmedHandler(
            ManagementDbContext managementDbContext,
            ILogger<OrderConfirmedHandler> logger,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            INotificationSenderService notificationSenderService)
            : base(logger, managementDbContext)
        {
            _logger = logger;
            _pricedOrderClient = pricedOrderClient;
            _notificationSenderService = notificationSenderService;
        }

        public async Task Consume(
            ConsumeContext<IOrderConfirmed> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                if (!await ProcessOrder(o => o.Id == @event.Message.SourceId, async o =>
                {
                    o.Status = OrderStatus.Paid;
                    o.PaidDateTime = DateTime.UtcNow;
                    o.ConfirmedOn = DateTime.UtcNow;

                    o.OwnerEmail = @event.Message.Email;
                    await ManagementDbContext.SaveChangesAsync();

                    var queryBuilder = new QueryBuilder()
                        .Equalz(
                            "order-id",
                            o.Id.ToString())
                        .Includes("priced-order-lines");

                    var pricedOder = await _pricedOrderClient
                        .GetListAsync(queryBuilder);

                    Logger.LogInformation($"We have an event instance ID of {o.EventInstanceId}");
                    var eventInstance = ManagementDbContext.EventInstances.First(x => x.Id == o.EventInstanceId);

                    var pricedOrders = pricedOder as PricedOrder[] ?? pricedOder.ToArray();

                    // somewhere around here there's no email address?

                    await _notificationSenderService.SendOrderConfirmedAsync(
                        o.OwnerEmail,
                        eventInstance,
                        pricedOrders.First(),
                        o);

                    await _notificationSenderService.SendOrderConfirmedOperatorAsync(
                        eventInstance,
                        pricedOrders.First(),
                        o);
                }))
                    Logger.LogError("Failed to locate the order with {0} to apply confirmed payment.",
                        @event.Message.SourceId);
            }
        }
    }
}