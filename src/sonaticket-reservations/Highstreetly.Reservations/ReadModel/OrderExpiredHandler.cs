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
    public class OrderExpiredHandler : DraftOrderReadModelHandlerBase<OrderExpiredHandler>, IConsumer<IOrderExpired>
    {
        private readonly ILogger<OrderExpiredHandler> _logger;
        private readonly ReservationDbContext _reservationDbContext;
        private RetryPolicy _waitForOrder;

        public OrderExpiredHandler(
            ReservationDbContext reservationDbContext,
            ILogger<OrderExpiredHandler> logger)
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
            ConsumeContext<IOrderExpired> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                var dto = _waitForOrder.Execute(()=> _reservationDbContext.DraftOrders.Single(x => x.Id == @event.Message.SourceId));
                if (WasNotAlreadyHandled(dto, @event.Message.Version))
                {
                    dto.State = States.Expired;
                    dto.OrderVersion = @event.Message.Version;
                    await _reservationDbContext.SaveChangesAsync();
                }
            }
        }
    }
}