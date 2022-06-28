using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Resources;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Order = Highstreetly.Management.Resources.Order;


namespace Highstreetly.Management.ReadModel
{
    public class ChargeRefundedHandler : IConsumer<IChargeRefunded>
    {
        private readonly ILogger<ChargeRefundedHandler> _logger;
       
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly ManagementDbContext _managementDbContext;
        private readonly INotificationSenderService _notificationSenderService;
        private readonly IAzureStorage _azureStorage;
        
        public ChargeRefundedHandler(
            ILogger<ChargeRefundedHandler> logger,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            ManagementDbContext managementDbContext,
            INotificationSenderService notificationSenderService,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _pricedOrderClient = pricedOrderClient;
            _managementDbContext = managementDbContext;
            _notificationSenderService = notificationSenderService;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IChargeRefunded> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var charge = JsonConvert.DeserializeObject<Charge>(stripeEvent);

                long refunded = 0;
                
                foreach (var chargeRefund in charge.Refunds)
                {
                    refunded += chargeRefund.Amount;
                }
                
                _logger.LogInformation($"Consuming {nameof(IChargeRefunded)}");
                var order = _managementDbContext.Find<Order>(context.Message.OrderId);

                order.RefundedDateTime = DateTime.UtcNow;
                
                order.Refunded = true;

                var queryBuilder = new QueryBuilder()
                    .Equalz(
                        "order-id",
                        order.Id.ToString())
                    .Includes("priced-order-lines");

                var pricedOrder = await _pricedOrderClient.GetListAsync(queryBuilder);

                var eventInstance = _managementDbContext
                    .EventInstances
                    .FirstOrDefault(x => x.Id == order.EventInstanceId);
                
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == eventInstance.EventSeriesId);
                var stats = _managementDbContext.DashboardStats.First(
                    x => x.EventOrganiserId == series.EventOrganiserId);

                var nowYear = DateTime.UtcNow.Year;
                var nowMonth = DateTime.UtcNow.Month;
                var nowDay = DateTime.UtcNow.Day;

                var refundsByDay = await GetRefundsProcessedByDay(stats.Id, series.EventOrganiserId, series.Id,
                    eventInstance.Id, nowYear, nowMonth, nowDay);

                refundsByDay.Total += 1;
                refundsByDay.TotalFunds += refunded;

                var byDay = await GetTicketSoldByDay(stats.Id, series.EventOrganiserId, series.Id, eventInstance.Id,
                    nowYear, nowMonth, nowDay);
                
                byDay.TotalFunds -= refunded;

                await _managementDbContext.SaveChangesAsync();

                await _eventOrganiserSiglnalrService.Send(series.EventOrganiserId.ToString(),
                    JsonConvert.SerializeObject(new
                    {
                        Status = SignalrConstants.OrderRefunded,
                        OrderId = order.Id,
                        EventInstanceId = eventInstance.Id
                    }));

                await _notificationSenderService.SendOrderRefundedEmail(
                    order.OwnerEmail,
                    eventInstance,
                    pricedOrder.First(),
                    order.HumanReadableId,
                    context.Message.ReceiptUrl,
                    refunded);
            }
        }

        private async Task<RefundsByDay> GetRefundsProcessedByDay(
                Guid statsId,
                Guid eventOrganiserId,
                Guid eventSeriesId,
                Guid eventInstanceId,
                int year,
                int month,
                int day)
        {
            var byDay = _managementDbContext.RefundsByDay
                .FirstOrDefault(x =>
                    x.Year == year && x.Month == month && x.Day == day && x.DashboardStatId == statsId &&
                    x.EventInstanceId == eventInstanceId && x.EventOrganiserId == eventOrganiserId &&
                    x.EventSeriesId == eventSeriesId);

            if (byDay == null)
            {
                byDay = new RefundsByDay
                {
                    Month = month,
                    Day = day,
                    Year = year,
                    EventOrganiserId = eventOrganiserId,
                    EventSeriesId = eventSeriesId,
                    EventInstanceId = eventInstanceId,
                    DashboardStatId = statsId
                };

                _managementDbContext.RefundsByDay.Add(byDay);
                await _managementDbContext.SaveChangesAsync();
            }

            return byDay;
        }

        public async Task<TicketsSoldByDay> GetTicketSoldByDay(
            Guid statsId,
            Guid eventOrganiserId,
            Guid eventSeriesId,
            Guid eventInstanceId,
            int year,
            int month,
            int day)
        {
            var byDay = _managementDbContext.TicketsSoldByDay.FirstOrDefault(x =>
                x.Year == year &&
                x.Month == month &&
                x.Day == day &&
                x.DashboardStatId == statsId &&
                x.EventInstanceId == eventInstanceId &&
                x.EventOrganiserId == eventOrganiserId &&
                x.EventSeriesId == eventSeriesId);

            if (byDay == null)
            {
                byDay = new TicketsSoldByDay
                {
                    Month = month,
                    Day = day,
                    Year = year,
                    EventInstanceId = eventInstanceId,
                    EventOrganiserId = eventOrganiserId,
                    EventSeriesId = eventSeriesId,
                    DashboardStatId = statsId
                };

                _managementDbContext.Add(byDay);
                await _managementDbContext.SaveChangesAsync();
            }

            return byDay;
        }
    }
}
