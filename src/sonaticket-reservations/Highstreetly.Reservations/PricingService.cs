using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations
{
    public class PricingService : IPricingService
    {
        private readonly ICachingJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        private readonly ILogger<PricingService> _logger;
        private readonly ReservationDbContext _reservationDbContext;
        private readonly ICachingJsonApiClient<TicketType, Guid> _ticketTypeClient;
        private readonly RetryPolicy _waitForOrder;

        public PricingService(
            ICachingJsonApiClient<EventInstance, Guid> eventInstanceClient,
            ICachingJsonApiClient<TicketType, Guid> ticketTypeClient,
            ILogger<PricingService> logger,
            ReservationDbContext reservationDbContext)
        {
            _eventInstanceClient = eventInstanceClient;
            _logger = logger;
            _reservationDbContext = reservationDbContext;
            _ticketTypeClient = ticketTypeClient;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2),
                                              TimeSpan.FromSeconds(3),
                                          });
        }

        public async Task<OrderTotal> CalculateTotal(
            Guid serviceId,
            Guid orderId,
            List<TicketQuantity> seatItems)
        {
            if (!seatItems.Any())
            {
                return new OrderTotal();
            }

            var seatTypesFromDatabase = new List<TicketType>();
            long deliveryFee = 0;

            foreach (var guid in seatItems.Select(x => x.TicketType))
                //TODO: this needs some pretty heavy caching
            {
                seatTypesFromDatabase.Add(await _ticketTypeClient.GetAsync(
                    guid,
                    new QueryBuilder().Includes("product-extra-groups,product-extra-groups.product-extras")));
            }

            var ei = await _eventInstanceClient.GetAsync(serviceId, new QueryBuilder().Includes("event-organiser"));

            var organiser = ei.EventOrganiser;
            var draftOrder = _waitForOrder.Execute(() => _reservationDbContext.DraftOrders.First(x => x.Id == orderId));

            var lineItems = new List<TicketOrderLine>();

            foreach (var item in seatItems)
            {
                var seatType = seatTypesFromDatabase.FirstOrDefault(x => x.Id == item.TicketType);
                if (seatType == null)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                        "Invalid seat type ID '{0}' for conference with ID '{1}'", item.TicketType, serviceId));
                }

                var unitPrice = PriceUnit(item, seatType);

                lineItems.Add(new TicketOrderLine
                {
                    TicketType = item.TicketType, Quantity = item.Quantity, UnitPrice = unitPrice,
                    LineTotal = unitPrice * item.Quantity
                });
            }

            var pricedOrderTotal = lineItems.Sum(x => x.LineTotal);
            var stripeFees = pricedOrderTotal * (long) .014 + 20;
            var platformFee = organiser.PlatformFee;

            _logger.LogDebug(
                $"ei.IsNationalDelivery: {ei.IsNationalDelivery} - draftOrder.IsNationalDelivery: {draftOrder.IsNationalDelivery}");

            if (ei.IsNationalDelivery && draftOrder.IsNationalDelivery)
            {
                if (ei.NationalDeliveryFlatFeeFreeAfter != null)
                {
                    _logger.LogDebug(
                        $"pricedOrderTotal: {pricedOrderTotal} - ei.NationalDeliveryFlatFeeFreeAfter: {ei.NationalDeliveryFlatFeeFreeAfter}");
                    if (pricedOrderTotal <= ei.NationalDeliveryFlatFeeFreeAfter)
                    {
                        _logger.LogDebug($"ei.NationalDeliveryFlatFee: {ei.NationalDeliveryFlatFee}");
                        if (ei.NationalDeliveryFlatFee != null)
                        {
                            deliveryFee = ei.NationalDeliveryFlatFee.Value;
                        }
                    }
                }
                else if (ei.NationalDeliveryFlatFee != null)
                {
                    deliveryFee = ei.NationalDeliveryFlatFee.Value;
                }
            }

            var total = pricedOrderTotal;

            switch (ei.PlatformFeePaidBy)
            {
                case 1:
                    // the fee comes out of our account
                    break;

                case 2:
                    total += platformFee ?? default;
                    break;

                case 3:
                    //total += (long)platformFee;
                    break;
            }

            switch (ei.PaymentPlatformFeePaidBy)
            {
                case 1:
                    // the fee comes out of our account
                    break;

                case 2:
                    total += stripeFees;
                    break;

                case 3:
                    //total += (long)stripeFees;
                    break;
            }

            _logger.LogInformation(
                $"pricing: {JsonConvert.SerializeObject(new {pricedOrderTotal, stripeFees, platformFee, deliveryFee})}");

            return new OrderTotal
            {
                Total = total + deliveryFee,
                Lines = lineItems,
                PaymentPlatformFees = stripeFees,
                PlatformFees = platformFee.GetValueOrDefault(),
                DeliveryFee = deliveryFee
            };
        }

        private long PriceUnit(
            TicketQuantity item,
            TicketType seatTypeFromDatabase)
        {
            if (item.TicketDetails == null)
            {
                return 0;
            }

            long extrasPrice = 0;

            if (item.TicketDetails.ProductExtras != null)
            {
                var selected = from eg in item.TicketDetails.ProductExtras
                    where eg.Selected
                    select eg;

                var allSelectedIds = selected.Select(x => x.ReferenceProductExtraId)
                    .ToList();

                extrasPrice = (from peg in seatTypeFromDatabase.ProductExtraGroups
                    from eg in peg.ProductExtras.Data
                    where allSelectedIds.Contains(eg.Id)
                    select
                        eg.Price * item
                            .TicketDetails
                            .ProductExtras
                            .Where(x => x.ReferenceProductExtraId == eg.Id)
                            .Sum(x => x.ItemCount)).Sum();
            }

            return extrasPrice + seatTypeFromDatabase.Price;
        }
    }
}