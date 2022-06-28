using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderTotalsCalculatedHandler : IConsumer<IOrderTotalsCalculated>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly ILogger<OrderTotalsCalculatedHandler> _logger;
        private readonly AsyncRetryPolicy _waitForOrder;

        public OrderTotalsCalculatedHandler(ReservationDbContext reservationDbContext, ILogger<OrderTotalsCalculatedHandler> logger)
        {
            _reservationDbContext = reservationDbContext;
            _logger = logger;

            _waitForOrder = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                });
        }

        /// <summary>
        /// the problem here is probably that when we are modifying an existing order 
        /// we will be creating a new priced order each time
        /// we should be checking to see if there is already a priced order before creating a new one
        /// </summary>
        /// <returns>The consume.</returns>
        /// <param name="event">Event.</param>
        public async Task Consume(
            ConsumeContext<IOrderTotalsCalculated> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                try
                {
                    var pricedOrder = await GetPricedOrder(@event.Message.SourceId);

                    if (!WasNotAlreadyHandled(pricedOrder, @event.Message.Version))
                    {
                        // message already handled, skip.
                        return;
                    }

                    if (@event.Message.Tickets != null)
                    {
                        var position = 0;
                        pricedOrder.PricedOrderLines.Clear();
                        await _reservationDbContext.SaveChangesAsync();

                        foreach (var ticketQuantity in @event.Message.Tickets)
                        {
                            var line = new PricedOrderLine
                            {
                                LineTotal = ticketQuantity.TicketDetails.Price * ticketQuantity.Quantity,
                                Position = position,
                                Description = ticketQuantity.TicketDetails.DisplayName,
                                UnitPrice = ticketQuantity.TicketDetails.Price,
                                ProductExtras = ticketQuantity
                                    .TicketDetails
                                    .ProductExtras
                                    .Select(x => new ProductExtra
                                    {
                                        Description = x.Description,
                                        Name = x.Name,
                                        Price = x.Price,
                                        Selected = x.Selected,
                                        ItemCount = x.ItemCount,
                                        ReferenceProductExtraId = x.ReferenceProductExtraId
                                    })
                                    .ToList(),
                                Quantity = ticketQuantity.Quantity,
                                Name = ticketQuantity.TicketDetails.Name,
                                TicketType = ticketQuantity.TicketType,
                                PricedOrder = pricedOrder
                            };

                            pricedOrder.PricedOrderLines.Add(line);

                            position++;
                        }
                    }

                    pricedOrder.Total = @event.Message.Total;
                    pricedOrder.IsFreeOfCharge = @event.Message.IsFreeOfCharge;
                    pricedOrder.OrderVersion = @event.Message.Version;
                    pricedOrder.PaymentPlatformFees = @event.Message.PaymentPlatformFees;
                    pricedOrder.PlatformFees = @event.Message.PlatformFees;
                    pricedOrder.OrderIsPriced = true;
                    pricedOrder.DeliveryFee = @event.Message.DeliveryFee;

                    await _reservationDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run IOrderTotalsCalculated  {@event.Message.SourceId} {ex.ToString()}",
                        ex);
                    throw;
                }
            }
        }

        private async Task<PricedOrder> GetPricedOrder(Guid id)
        {
            return await _waitForOrder.ExecuteAsync(async ()=> await _reservationDbContext
                .PricedOrders
                .Include(x => x.PricedOrderLines)
                .SingleAsync(x => x.OrderId == id));
        }

        private static bool WasNotAlreadyHandled(PricedOrder pricedOrder, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > pricedOrder.OrderVersion)
            {
                return true;
            }
            else if (eventVersion == pricedOrder.OrderVersion)
            {
                Trace.TraceWarning(
                    "Ignoring duplicate priced order update message with version {1} for order id {0}",
                    pricedOrder.OrderId,
                    eventVersion);
                return false;
            }
            else
            {
                Trace.TraceWarning(
                    @"Ignoring an older order update message was received with with version {1} for order id {0}, last known version {2}.
                        This read model generator has an expectation that the EventBus will deliver messages for the same source in order. Nevertheless, this warning can be expected in a migration scenario.",
                    pricedOrder.OrderId,
                    eventVersion,
                    pricedOrder.OrderVersion);
                return false;
            }
        }
    }
}