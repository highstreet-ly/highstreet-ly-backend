using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Management.ReadModel
{
    public class OrderProcessingCompletedHandler : IConsumer<IOrderProcessingCompleted>
    {
        private readonly ILogger<OrderProcessingCompletedHandler> _logger;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly ManagementDbContext _managementDbContext;

        private readonly INotificationSenderService _notificationSenderService;

        public OrderProcessingCompletedHandler(
            ILogger<OrderProcessingCompletedHandler> logger,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            ManagementDbContext managementDbContext,
            INotificationSenderService notificationSenderService)
        {
            _logger = logger;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _pricedOrderClient = pricedOrderClient;
            _managementDbContext = managementDbContext;
            _notificationSenderService = notificationSenderService;
        }

        public async Task Consume(
            ConsumeContext<IOrderProcessingCompleted> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation(
                    $"Consuming IOrderProcessingCompleted for {@context.Message.OrderId} setting status to {OrderStatus.ProcessingComplete}");

                var order = _managementDbContext.Orders.First(x => x.Id == @context.Message.OrderId);
                order.Status = OrderStatus.ProcessingComplete;

                await _managementDbContext.SaveChangesAsync();

                var eventInstance =
                    _managementDbContext.EventInstances.First(x => x.Id == order.EventInstanceId);

                var queryBuilder = new QueryBuilder()
                    .Equalz(
                        "order-id",
                        order.Id.ToString())
                    .Includes("priced-order-lines");

                var pricedOrder = await _pricedOrderClient.GetListAsync(queryBuilder);

                await _eventOrganiserSiglnalrService.Send(eventInstance.EventOrganiserId.ToString(), JsonConvert.SerializeObject(new
                {
                    Status = SignalrConstants.OrderProcessingComplete,
                    OrderId = order.Id,
                    EventInstanceId = eventInstance.Id
                }));

                await _notificationSenderService.SendOrderProcessingCompleteAsync(
                    order.OwnerEmail,
                    eventInstance,
                    pricedOrder.First(),
                    order);
            }
        }
    }
}