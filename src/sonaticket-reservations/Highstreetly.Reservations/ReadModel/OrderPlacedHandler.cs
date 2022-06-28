using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Contracts.Requests;
using Highstreetly.Reservations.Resources;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using PricedOrder = Highstreetly.Reservations.Resources.PricedOrder;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPlacedHandler : IConsumer<IOrderPlaced>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly IJsonApiClient<User, Guid> _userClient;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        private readonly ILogger<OrderPlacedHandler> _logger;
        private readonly RetryPolicy _waitForOrder;

        public OrderPlacedHandler(
            ReservationDbContext reservationDbContext,
            IJsonApiClient<User, Guid> userClient,
            IJsonApiClient<EventInstance, Guid> eventInstanceClient,
            ILogger<OrderPlacedHandler> logger)
        {
            _reservationDbContext = reservationDbContext;
            _userClient = userClient;
            _eventInstanceClient = eventInstanceClient;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2),
                                              TimeSpan.FromSeconds(3),
                                          });
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IOrderPlaced> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                // I am unsure that we should be doing this here as there is also a draft order being 
                // saved in DraftOrderViewModelService when POST
                // although we seem to only come in here if there is no draft order when handling IAddTicketsToBasket
                _logger.LogInformation($"handling IOrderPlaced for {@event.Message.SourceId}");

                User user = null;
                if (@event.Message.OwnerId != Guid.Empty)
                {
                    user = await _userClient.GetAsync(@event.Message.OwnerId.GetValueOrDefault());
                }

                var draftOrder = _waitForOrder.Execute(() => _reservationDbContext.DraftOrders.First(x => x.Id == @event.Message.SourceId));
                draftOrder.State = States.PendingReservation;

                if (user != null)
                {
                    draftOrder.OwnerEmail = user.Email;
                }

                draftOrder.OwnerId = @event.Message.OwnerId;
                draftOrder.OrderVersion = @event.Message.Version;

                await _reservationDbContext.SaveChangesAsync();

                // process the priced order

                if (!_reservationDbContext.PricedOrders.Any(x => x.Id == @event.Message.SourceId))
                {
                    var pricedOrder = new PricedOrder
                    {
                        OrderId = @event.Message.SourceId,
                        ReservationExpirationDate = @event.Message.ReservationAutoExpiration,
                        OrderVersion = @event.Message.Version,
                        OwnerId = @event.Message.OwnerId,
                        HumanReadableId = @event.Message.HumanReadableId,
                        EventInstanceId = @event.Message.EventInstanceId
                    };
                    
                    await _reservationDbContext.PricedOrders.AddAsync(pricedOrder);

                    await _reservationDbContext.SaveChangesAsync();
                }
            }
        }
    }
}