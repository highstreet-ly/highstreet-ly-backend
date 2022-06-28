using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations.Domain
{
    public class Order : EventSourced
    {
        private static readonly TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(60);
        public Guid ConferenceId;
        private bool _isConfirmed;
        private List<TicketQuantity> _seats;

        public Order()
        {
            Handles<OrderPlaced>(Apply);
            Handles<OrderUpdated>(Apply);
            Handles<OrderPartiallyReserved>(Apply);
            Handles<OrderReservationCompleted>(Apply);
            Handles<OrderExpired>(Apply);
            Handles<OrderPaymentConfirmed>(e => Apply(new OrderConfirmed
            {
                Delay = e.Delay,
                SourceId = e.SourceId,
                Version = e.Version
            }));
            Handles<OrderConfirmed>(Apply);
            Handles<OrderRegistrantAssigned>(Apply);
            Handles<OrderTotalsCalculated>(Apply);
            Handles<OrderProcessingStarted>(Apply);
            Handles<OrderProcessingCompleted>(Apply);
            Handles<DeliveryMethodChanged>(Apply);
        }

        public Order(Guid id, IEnumerable<ISonaticketEvent> history) : this()
        {
            this.Id = id;
            this.LoadFromHistory(history);
        }

        public Order(
            Guid id,
            Guid conferenceId,
            IEnumerable<OrderItem> items,
            Guid ownerId,
            string ownerEmail,
            Guid correlation,
            string humanReadableId,
            bool isClickAndCollect,
            bool isLocalDelivery,
            bool isNationalDelivery,
            bool isToTable,
            string tableInfo) : this()
        {
            Id = id;
            CorrelationId = correlation;
            HumanReadableId = humanReadableId;

            items ??= new List<OrderItem>();

            var orderItems = items as OrderItem[] ?? items.ToArray();

            var all = ConvertItems(orderItems);
            Update<IOrderPlaced>(new OrderPlaced
            {
                OwnerEmail = ownerEmail,
                EventInstanceId = conferenceId,
                Tickets = all.ToList(),
                Version = Version,
                ReservationAutoExpiration = DateTime.UtcNow.Add(ReservationAutoExpiration),
                OwnerId = ownerId,
                CorrelationId = CorrelationId,
                HumanReadableId = humanReadableId,
                IsClickAndCollect = isClickAndCollect,
                IsLocalDelivery = isLocalDelivery,
                IsNationalDelivery = isNationalDelivery,
                IsToTable = isToTable,
                TableInfo = tableInfo
            });
        }

        public string HumanReadableId { get; set; }

        public void UpdateSeats(IEnumerable<OrderItem> items, OrderTotal totals)
        {
            var all = ConvertItems(items);

            Update<IOrderUpdated>(new OrderUpdated { Tickets = all, CorrelationId = CorrelationId, Version = Version });

            if (totals.Total > 0)
            {
                Update<IOrderTotalsCalculated>(new OrderTotalsCalculated
                                               {
                                                   EventInstanceId = ConferenceId,
                                                   Tickets = all.ToList(),
                                                   ReservationAutoExpiration =
                                                       DateTime.UtcNow.Add(ReservationAutoExpiration),
                                                   Total = totals.Total,
                                                   Lines = totals.Lines?.ToArray(),
                                                   IsFreeOfCharge = totals.Total == 0m,
                                                   PaymentPlatformFees = totals.PaymentPlatformFees,
                                                   PlatformFees = totals.PlatformFees,
                                                   CorrelationId = CorrelationId,
                                                   DeliveryFee = totals.DeliveryFee,
                                                   Version = Version
                                               });
            }
        }

        public void MarkAsReserved(OrderTotal totals,
                                   DateTime expirationDate,
                                   IEnumerable<TicketQuantity> reservedSeats, 
                                   Guid orderConferenceId)
        {
            if (_isConfirmed)
                throw new InvalidOperationException("Cannot modify a confirmed order.");

            var reserved = reservedSeats.ToList();

            // Is there an order item which didn't get an exact reservation?
            if (_seats.Any(item =>
                item.Quantity != 0 && !reserved.Any(seat =>
                    seat.TicketType == item.TicketType && seat.Quantity == item.Quantity)))
            {
                Update<IOrderPartiallyReserved>(new OrderPartiallyReserved
                {
                    ReservationExpiration = expirationDate,
                    Tickets = reserved.ToArray(),
                    CorrelationId = CorrelationId,
                    Version = Version
                });
                // if (totals.Total > 0)
                // {
                //     Update<IOrderTotalsCalculated>(new OrderTotalsCalculated
                //                                    {
                //                                        EventInstanceId = orderConferenceId,
                //                                        Total = totals.Total,
                //                                        Lines = totals.Lines.ToArray(),
                //                                        IsFreeOfCharge = totals.Total == 0m,
                //                                        PaymentPlatformFees = totals.PaymentPlatformFees,
                //                                        PlatformFees = totals.PlatformFees,
                //                                        CorrelationId = CorrelationId,
                //                                        DeliveryFee = totals.DeliveryFee,
                //                                        Version = Version,
                //                                        Tickets = reserved.ToArray(),
                //                                    });
                // }
            }
            else
            {
                Update<IOrderReservationCompleted>(new OrderReservationCompleted
                {
                    ReservationExpiration = expirationDate,
                    Tickets = reserved.ToArray(),
                    CorrelationId = CorrelationId,
                    Version = Version
                });
                // if (totals.Total > 0)
                // {
                //     Update<IOrderTotalsCalculated>(new OrderTotalsCalculated
                //                                    {
                //                                        EventInstanceId = orderConferenceId,
                //                                        Total = totals.Total,
                //                                        Lines = totals.Lines.ToArray(),
                //                                        IsFreeOfCharge = totals.Total == 0m,
                //                                        PaymentPlatformFees = totals.PaymentPlatformFees,
                //                                        PlatformFees = totals.PlatformFees,
                //                                        CorrelationId = CorrelationId,
                //                                        DeliveryFee = totals.DeliveryFee,
                //                                        Version = Version,
                //                                        Tickets = reserved.ToArray(),
                //                                    });
                // }
            }
        }

        public void Expire()
        {
            if (_isConfirmed)
                throw new InvalidOperationException("Cannot expire a confirmed order.");

            Update<IOrderExpired>(new OrderExpired()
            {
                CorrelationId = CorrelationId,
                Version = Version
            });
        }

        public void Confirm(string email)
        {
            Update<IOrderConfirmed>(new OrderConfirmed
            {
                Email = email,
                CorrelationId = CorrelationId,
                Version = Version
            });
        }

        public void AssignRegistrant(string ownerName, string email, Guid userId, string phone, string deliveryLine1, string deliveryPostcode)
        {
            Update<IOrderRegistrantAssigned>(new OrderRegistrantAssigned
            {
                OwnerName = ownerName,
                Email = email,
                UserId = userId,
                CorrelationId = CorrelationId,
                Version = Version,
                DeliveryLine1 = deliveryLine1,
                DeliveryPostcode = deliveryPostcode,
                Phone = phone,
                SourceId = Id
            });
        }

        public static List<TicketQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            if (items == null) return new List<TicketQuantity>();
            return items.Select(x => new TicketQuantity(x.TicketType, x.Quantity, new OrderTicketDetails
            {
                Id = x.Ticket.Id,
                Name = x.Ticket.Name,
                Price = x.Ticket.Price,
                Quantity = x.Ticket.Quantity,
                DisplayName = x.Ticket.DisplayName,
                EventInstanceId = x.Ticket.EventInstanceId,
                ProductExtras = x.Ticket.ProductExtras.Select(pe => new ProductExtra
                {
                    Description = pe.Description,
                    Name = pe.Name,
                    Price = pe.Price,
                    Selected = pe.Selected,
                    Id = pe.Id,
                    ItemCount = pe.ItemCount,
                    ReferenceProductExtraId = pe.ReferenceProductExtraId
                }).ToList()
            })).ToList();
        }

        public void Apply(OrderPlaced e)
        {
            ConferenceId = e.EventInstanceId;
            _seats = e.Tickets.ToList();
        }

        public void Apply(OrderUpdated e)
        {
            _seats = e.Tickets.ToList();
        }

        public void Apply(OrderPartiallyReserved e)
        {
            _seats = e.Tickets.ToList();
        }

        public void Apply(OrderReservationCompleted e)
        {
            _seats = e.Tickets.ToList();
        }

        public void Apply(OrderExpired e)
        {
        }

        public void Apply(OrderConfirmed e)
        {
            _isConfirmed = true;
        }

        public void Apply(OrderRegistrantAssigned e)
        {
        }

        public void Apply(OrderTotalsCalculated e)
        {
            var x = e;
        }

        private void Apply(OrderProcessingStarted e)
        {
        }

        private void Apply(DeliveryMethodChanged e)
        {
        }


        private void Apply(OrderProcessingCompleted e)
        {
        }

        public void Processing()
        {
            if (!_isConfirmed)
                throw new InvalidOperationException(
                    "Cannot set to processing an order that isn't confirmed yet.");

            Update<IOrderProcessingStarted>(new OrderProcessingStarted
            {
                OrderId = this.Id,
                Version = Version
            });
        }

        public void ProcessingComplete()
        {
            if (!_isConfirmed)
                throw new InvalidOperationException(
                    "Cannot set to processing an order that isn't confirmed yet.");

            Update<IOrderProcessingCompleted>(new OrderProcessingCompleted
            {
                OrderId = this.Id,
                Version = Version
            });
        }

        public void SetDeliveryMethod(List<OrderItem> items, OrderTotal totals)
        {
            var all = ConvertItems(items);

            // Update<IOrderUpdated>(new OrderUpdated { Tickets = all, CorrelationId = CorrelationId, Version = Version });

            if (totals.Total > 0)
            {
                Update<IOrderTotalsCalculated>(new OrderTotalsCalculated
                                               {
                                                   EventInstanceId = ConferenceId,
                                                   Tickets = all.ToList(),
                                                   ReservationAutoExpiration = DateTime.UtcNow.Add(ReservationAutoExpiration),
                                                   Total = totals.Total,
                                                   Lines = totals.Lines?.ToArray(),
                                                   IsFreeOfCharge = totals.Total == 0m,
                                                   PaymentPlatformFees = totals.PaymentPlatformFees,
                                                   PlatformFees = totals.PlatformFees,
                                                   CorrelationId = CorrelationId,
                                                   DeliveryFee = totals.DeliveryFee,
                                                   Version = Version
                                               });
            }

            Update<IDeliveryMethodChanged>(new DeliveryMethodChanged
            {
                SourceId = Id,
                Version = Version,
                CorrelationId = CorrelationId
            });
        }
    }
}