using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Management.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Highstreetly.Management.ReadModel
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrderTotalsCalculatedHandler : IConsumer<IOrderTotalsCalculated>
    {
        private readonly ILogger<OrderTotalsCalculatedHandler> _logger;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly IUserSiglnalrService _userSiglnalrService;
        private readonly ManagementDbContext _managementDbContext;
        private readonly AsyncRetryPolicy _waitForOrder;

        public OrderTotalsCalculatedHandler(
            ILogger<OrderTotalsCalculatedHandler> logger,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            IUserSiglnalrService userSiglnalrService,
            ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _userSiglnalrService = userSiglnalrService;
            _managementDbContext = managementDbContext;
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
        ///     the problem here is probably that when we are modifying an existing order
        ///     we will be creating a new priced order each time
        ///     we should be checking to see if there is already a proceed order before creating a new one
        /// </summary>
        /// <returns>The consume.</returns>
        /// <param name="event">Event.</param>
        public async Task Consume(
            ConsumeContext<IOrderTotalsCalculated> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation(
                    $"Consuming IOrderTotalsCalculated for {@event.Message.SourceId} setting status to {OrderStatus.Priced}");

                var order = await _waitForOrder.ExecuteAsync(async ()=> await _managementDbContext.Orders.FirstAsync(x => x.Id == @event.Message.SourceId));

                if (order != null) // might be a brand new order 
                {
                    order.Status = OrderStatus.Priced;
                    order.PricedDateTime = DateTime.UtcNow;
                    await _managementDbContext.SaveChangesAsync();
                }

                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == order.EventInstanceId);
                // todo: this (organiser.Id) should come from the command 
                var organiser =
                    _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == eventInstance.EventOrganiserId);

                await _eventOrganiserSiglnalrService.Send(organiser.Id.ToString(), JsonConvert.SerializeObject(new
                {
                    Status = SignalrConstants.OrderPriced,
                    OrderId = order.Id,
                    EventInstanceId = eventInstance.Id
                }));

                await _userSiglnalrService.Send(order.OwnerId.ToString(), SignalrConstants.OrderPriced);
            }
        }
    }
}