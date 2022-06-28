using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using PricedOrder = Highstreetly.Reservations.Resources.PricedOrder;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderConfirmedHandler : DraftOrderConfirmedReadModelHandlerBase<OrderConfirmedHandler>,
        IConsumer<IOrderConfirmed>
    {
        private readonly ILogger<OrderConfirmedHandler> _logger;
        private readonly AsyncRetryPolicy _waitForOrder;

        public OrderConfirmedHandler(ReservationDbContext reservationDbContext, ILogger<OrderConfirmedHandler> logger) :
            base(reservationDbContext)
        {
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

        public async Task Consume(
            ConsumeContext<IOrderConfirmed> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                await ConsumeOrderConfirmed(@event.Message);

                try
                {
                    var pricedOrder = await GetPricedOrder(@event.Message.SourceId);
                    if (WasNotAlreadyHandled(pricedOrder, @event.Message.Version))
                    {
                        pricedOrder.ReservationExpirationDate = null;
                        pricedOrder.OrderVersion = @event.Message.Version;
                        await ReservationDbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run IOrderConfirmed  {@event.Message.SourceId}", ex);
                    throw;
                }

                await ReservationDbContext.SaveChangesAsync();
            }
        }

        private async Task<PricedOrder> GetPricedOrder(Guid id)
        {
           return await _waitForOrder.ExecuteAsync(async ()=> await ReservationDbContext.PricedOrders.SingleAsync(x => x.OrderId == id)) ;
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