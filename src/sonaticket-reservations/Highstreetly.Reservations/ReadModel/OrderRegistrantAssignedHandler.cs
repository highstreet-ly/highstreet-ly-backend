using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderRegistrantAssignedHandler
        : DraftOrderReadModelHandlerBase<OrderRegistrantAssignedHandler>,
            IConsumer<IOrderRegistrantAssigned>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly ILogger<OrderRegistrantAssignedHandler> _logger;
        private readonly RetryPolicy _waitForOrder;

        public OrderRegistrantAssignedHandler(
            ReservationDbContext reservationDbContext,
            ILogger<OrderRegistrantAssignedHandler> logger)
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
            ConsumeContext<IOrderRegistrantAssigned> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                var draftOrder = _waitForOrder.Execute(()=>_reservationDbContext.DraftOrders.Single(x => x.Id == @event.Message.SourceId)) ;

                if (WasNotAlreadyHandled(draftOrder, @event.Message.Version))
                {
                    draftOrder.OwnerEmail = @event.Message.Email;
                    draftOrder.OwnerId = @event.Message.UserId;
                    draftOrder.OrderVersion = @event.Message.Version;
                    await _reservationDbContext.SaveChangesAsync();
                }

                var dto =_waitForOrder.Execute(()=> _reservationDbContext.PricedOrders.Single(
                    x => x.OrderId == @event.Message.SourceId));

                dto.OwnerId = @event.Message.UserId;

                await _reservationDbContext.SaveChangesAsync();
            }
        }
    }
}