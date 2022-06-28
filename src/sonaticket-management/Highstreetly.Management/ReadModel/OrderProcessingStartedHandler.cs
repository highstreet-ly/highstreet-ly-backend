using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
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
    public class OrderProcessingStartedHandler : IConsumer<IOrderProcessingStarted>
    {
        private readonly ILogger<OrderProcessingStartedHandler> _logger;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly IEmailSender _emailSender;
        private readonly EmailTemplateOptions _emailTemplateOptions;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly ManagementDbContext _managementDbContext;
        private readonly INotificationSenderService _notificationSenderService;

        public OrderProcessingStartedHandler(
            ILogger<OrderProcessingStartedHandler> logger,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            IEmailSender emailSender,
            EmailTemplateOptions emailTemplateOptions,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            ManagementDbContext managementDbContext,
            INotificationSenderService notificationSenderService)
        {
            _logger = logger;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _emailSender = emailSender;
            _emailTemplateOptions = emailTemplateOptions;
            _pricedOrderClient = pricedOrderClient;
            _managementDbContext = managementDbContext;
            _notificationSenderService = notificationSenderService;
        }

        public async Task Consume(
            ConsumeContext<IOrderProcessingStarted> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation(
                    $"Consuming ISetOrderProcessing for {@context.Message.OrderId} setting status to {OrderStatus.Processing}");

                var order = _managementDbContext.Orders.FirstOrDefault(x => x.Id == @context.Message.OrderId);
                order.Status = OrderStatus.Processing;
                order.ProcessingDateTime = DateTime.UtcNow;
                await _managementDbContext.SaveChangesAsync();

                var queryBuilder = new QueryBuilder()
                    .Equalz(
                        "order-id",
                        order.Id.ToString())
                    .Includes("priced-order-lines");

                var pricedOder = await _pricedOrderClient.GetListAsync(queryBuilder);

                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == order.EventInstanceId);
                var organiser =
                    _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == eventInstance.EventOrganiserId);

                await _eventOrganiserSiglnalrService.Send(organiser.Id.ToString(), JsonConvert.SerializeObject(new
                {
                    Status = SignalrConstants.OrderProcessing,
                    OrderId = order.Id,
                    EventInstanceId = eventInstance.Id
                }));

                await _notificationSenderService.SendOrderProcessingAsync(
                    order.OwnerEmail,
                    eventInstance,
                    pricedOder.First(),
                    order);
            }
        }
    }
}