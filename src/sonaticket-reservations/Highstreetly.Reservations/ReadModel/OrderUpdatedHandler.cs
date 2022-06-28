using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderUpdatedHandler : DraftOrderReadModelHandlerBase<OrderUpdatedHandler>, IConsumer<IOrderUpdated>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly ILogger<OrderUpdatedHandler> _logger;
        private RetryPolicy _waitForOrder;

        public OrderUpdatedHandler(
            ReservationDbContext reservationDbContext,
            ILogger<OrderUpdatedHandler> logger)
        {
            _reservationDbContext = reservationDbContext;
            _logger = logger;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2),
                                              TimeSpan.FromSeconds(3),
                                          });
        }

        public async Task Consume(
            ConsumeContext<IOrderUpdated> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                var draftOrder = _waitForOrder.Execute(() => _reservationDbContext.DraftOrders.Single(x => x.Id == @event.Message.SourceId));

                if (WasNotAlreadyHandled(draftOrder, @event.Message.Version))
                {
                    // draftOrder.Lines.Clear();
                    // draftOrder.Lines.AddRange(@event.Message.Tickets.Select(seat => new DraftOrderItem
                    // {
                    //     TicketType =  seat.TicketType,
                    //     RequestedTickets = seat.Quantity,
                    //     Ticket =  seat.TicketDetails
                    // }).ToArray());
                    draftOrder.State = States.PendingReservation;
                    draftOrder.OrderVersion = @event.Message.Version;
                    await _reservationDbContext.SaveChangesAsync();
                }
            }
        }
    }
}