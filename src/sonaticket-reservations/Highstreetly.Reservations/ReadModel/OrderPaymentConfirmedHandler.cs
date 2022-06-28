using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPaymentConfirmedHandler : DraftOrderConfirmedReadModelHandlerBase<OrderPaymentConfirmedHandler>,
        IConsumer<IOrderPaymentConfirmed>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<OrderPaymentConfirmedHandler> _logger;

        public OrderPaymentConfirmedHandler(ReservationDbContext reservationDbContext, IMapper mapper,
            ILogger<OrderPaymentConfirmedHandler> logger) : base(reservationDbContext)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IOrderPaymentConfirmed> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                var message = _mapper.Map<IOrderConfirmed>(@event.Message);
                return ConsumeOrderConfirmed(message);
            }
        }
    }
}