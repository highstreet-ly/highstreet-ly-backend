using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderReservationCompletedHandler
        : DraftOrderUpdateReservedHandlerBase<OrderReservationCompletedHandler>, IConsumer<IOrderReservationCompleted>
    {
        private readonly ILogger<OrderReservationCompletedHandler> _logger;

        public OrderReservationCompletedHandler(
            ReservationDbContext reservationDbContext,
            ILogger<OrderReservationCompletedHandler> logger)
            : base(reservationDbContext)
        {
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IOrderReservationCompleted> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
                {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                return UpdateReserved(@event.Message.SourceId, @event.Message.ReservationExpiration,
                    States.ReservationCompleted, @event.Message.Version, @event.Message.Tickets);
            }
        }
    }
}