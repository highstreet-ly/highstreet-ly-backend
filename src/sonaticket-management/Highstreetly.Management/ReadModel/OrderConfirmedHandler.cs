using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Resources;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProductExtra = Highstreetly.Management.Resources.ProductExtra;

namespace Highstreetly.Management.ReadModel
{
    public class OrderConfirmedHandler :
        IConsumer<IOrderConfirmed>
    {
        private readonly IJsonApiClient<DraftOrder, Guid> _draftOrderClient;
        private readonly ILogger<OrderConfirmedHandler> _logger;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly ManagementDbContext _managementDbContext;

        public OrderConfirmedHandler(
            IJsonApiClient<DraftOrder, Guid> draftOrderClient,
            ILogger<OrderConfirmedHandler> logger,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            ManagementDbContext managementDbContext)
        {
            _draftOrderClient = draftOrderClient;
            _logger = logger;
            _pricedOrderClient = pricedOrderClient;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<IOrderConfirmed> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                // get the event organiser id
                _logger.LogInformation("Consuming IOrderConfirmed in OrderConfirmedHandler");

                var order = await _draftOrderClient.GetAsync(context.Message.SourceId);

                var queryBuilder = new QueryBuilder()
                    .Equalz(
                        "order-id",
                        context.Message.SourceId.ToString())
                    .Includes("priced-order-lines.product-extras");

                var pricedOrders = await _pricedOrderClient.GetListAsync(queryBuilder);

                var pricedOrder = pricedOrders.First();

                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == order.EventInstanceId);
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == eventInstance.EventSeriesId);
                var organiser = _managementDbContext
                    .EventOrganisers
                    .Include(x => x.DashboardStats)
                    .ThenInclude(x => x.OrdersProcessedByDay)
                    .Include(x => x.DashboardStats)
                    .ThenInclude(x => x.RefundsProcessedByDay)
                    .Include(x => x.DashboardStats)
                    .ThenInclude(x => x.RegisteredInterestByDay)
                    .Include(x => x.DashboardStats)
                    .ThenInclude(x => x.TicketsSoldByDay)
                    .FirstOrDefault(x => x.Id == series.EventOrganiserId);

                var stats = organiser.DashboardStats;

                if (stats == null)
                {
                    stats = new DashboardStat
                    {
                        Id = NewId.NextGuid(),
                        EventOrganiserId = organiser.Id
                    };

                    await _managementDbContext.DashboardStats.AddAsync(stats);
                }

                var confirmedInThisOrder = pricedOrder.PricedOrderLines.Sum(x => x.Quantity);

                stats.TotalOrdersFullfiled += 1;

                var nowYear = DateTime.UtcNow.Year;
                var nowMonth = DateTime.UtcNow.Month;
                var nowDay = DateTime.UtcNow.Day;

                var ordersByDay = GetOrdersProcessedByDay(stats, series.EventOrganiserId, series.Id,
                    eventInstance.Id, nowYear, nowMonth, nowDay);

                ordersByDay.Total += 1;

                var byDay = GetTicketSoldByDay(stats, series.EventOrganiserId, series.Id, eventInstance.Id,
                    nowYear, nowMonth, nowDay);
                byDay.Total += confirmedInThisOrder.GetValueOrDefault();

                // the revenue for the operator shouldn't include fees

                var operatorFundsForThisOrder = pricedOrder.Total.GetValueOrDefault() -
                                                pricedOrder.PaymentPlatformFees.GetValueOrDefault() -
                                                pricedOrder.PlatformFees.GetValueOrDefault();
                
                byDay.TotalFunds += operatorFundsForThisOrder;
                ordersByDay.TotalFunds += operatorFundsForThisOrder;

                var orderReadModel = _managementDbContext.Orders.FirstOrDefault(x => x.Id == context.Message.SourceId);

                orderReadModel.Tickets.Clear();

                var applicableTicketTypeIds = pricedOrder.PricedOrderLines.Select(x => x.TicketType)
                    .ToList();
                var applicableTicketTypes =
                    _managementDbContext
                        .TicketTypes
                        .Where(x => applicableTicketTypeIds.Contains(x.Id))
                        .ToList();

                var orderTickets = pricedOrder
                    .PricedOrderLines
                    .Select(x => new Resources.OrderTicket
                    {
                        OrderId = orderReadModel.Id,
                        TicketTypeConfigurationId = x.TicketType,
                        Id = NewId.NextGuid(),
                        TicketDetails = new OrderTicketDetails
                        {
                            Name = applicableTicketTypes.First(tt => tt.Id == x.TicketType)
                                .Name,
                            Price = x.UnitPrice.GetValueOrDefault(),
                            Quantity = x.Quantity.GetValueOrDefault(),
                            DisplayName = x.Description,
                            EventInstanceId = eventInstance.Id,
                            Id = NewId.NextGuid(),
                            ProductExtras = x.ProductExtras.Select(pe =>
                                    new ProductExtra
                                    {
                                        Description = pe.Description,
                                        Name = pe.Name,
                                        Price = pe.Price,
                                        Selected = pe.Selected,
                                        Id = NewId.NextGuid(),
                                        ItemCount = pe.ItemCount,
                                    })
                                .ToList()
                        }
                    });

                await _managementDbContext.OrderTickets.AddRangeAsync(orderTickets);

                await _managementDbContext.SaveChangesAsync();

                await _eventOrganiserSiglnalrService.Send(organiser.Id.ToString(), JsonConvert.SerializeObject(new
                {
                    Status = SignalrConstants.OrderConfirmed,
                    OrderId = order.Id,
                    EventInstanceId = eventInstance.Id
                }));
            }
        }

        public OrdersByDay GetOrdersProcessedByDay(DashboardStat stats, Guid eventOrganiserId,
            Guid eventSeriesId, Guid eventInstanceId,
            int year, int month, int day)
        {
           var byDay = stats.OrdersProcessedByDay
                .FirstOrDefault(x =>
                    x.Year == year && x.Month == month && x.Day == day && x.DashboardStatId == stats.Id &&
                    x.EventInstanceId == eventInstanceId && x.EventOrganiserId == eventOrganiserId &&
                    x.EventSeriesId == eventSeriesId);

            if (byDay == null)
            {
                byDay = new OrdersByDay
                {
                    Month = month,
                    Day = day,
                    Year = year,
                    EventOrganiserId = eventOrganiserId,
                    EventSeriesId = eventSeriesId,
                    EventInstanceId = eventInstanceId,
                    DashboardStatId = stats.Id
                };

               stats.OrdersProcessedByDay.Add(byDay);
            }

            return byDay;
        }

        public TicketsSoldByDay GetTicketSoldByDay(DashboardStat stats, Guid eventOrganiserId, Guid eventSeriesId,
            Guid eventInstanceId,
            int year, int month, int day)
        {
            var byDay = stats.TicketsSoldByDay
                .FirstOrDefault(x =>
                    x.Year == year &&
                    x.Month == month &&
                    x.Day == day &&
                    x.DashboardStatId == stats.Id &&
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
                    DashboardStatId = stats.Id
                };

                stats.TicketsSoldByDay.Add(byDay);
            }

            return byDay;
        }
    }
}