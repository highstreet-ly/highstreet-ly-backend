using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPartiallyReservedHandler
        : DraftOrderUpdateReservedHandlerBase<OrderPartiallyReservedHandler>,
            IConsumer<IOrderPartiallyReserved>
    {
        private readonly ILogger<OrderPartiallyReservedHandler> _logger;

        public OrderPartiallyReservedHandler(
            ReservationDbContext reservationDbContext,
            ILogger<OrderPartiallyReservedHandler> logger)
            : base(reservationDbContext)
        {
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IOrderPartiallyReserved> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                return UpdateReserved(@event.Message.SourceId, @event.Message.ReservationExpiration,
                    States.PartiallyReserved, @event.Message.Version, @event.Message.Tickets);
            }
        }
    }
}