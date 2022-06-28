using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Reservations.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;


namespace Highstreetly.Reservations.ReadModel
{
    public abstract class DraftOrderUpdateReservedHandlerBase<T> : DraftOrderReadModelHandlerBase<T>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly RetryPolicy _waitForOrder;

        protected DraftOrderUpdateReservedHandlerBase(ReservationDbContext reservationDbContext)
        {
            _reservationDbContext = reservationDbContext;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                               {
                                                   TimeSpan.FromSeconds(1),
                                                   TimeSpan.FromSeconds(2),
                                                   TimeSpan.FromSeconds(3),
                                               });
        }

        protected async Task UpdateReserved(Guid orderId, DateTime reservationExpiration, States state, int orderVersion, IEnumerable<TicketQuantity> tickets)
        {
            var draftOrder = _waitForOrder.Execute(()=>_reservationDbContext
                                                       .DraftOrders
                                                       .Include(x=>x.DraftOrderItems)
                                                       .ThenInclude(x=>x.Ticket)
                                                       .ThenInclude(x=>x.ProductExtras)
                                                       .Single(x=>x.Id == orderId));

            if (WasNotAlreadyHandled(draftOrder, orderVersion))
            {
                // this is a hack until we make the priced order come into existence later in the process
                var pricedOrder = _waitForOrder.Execute(()=> _reservationDbContext
                                                                  .PricedOrders
                                                                  .Include(x => x.PricedOrderLines)
                                                                  .Single(x => x.OrderId == orderId)) ;

                foreach (var seat in tickets)
                {
                    var items = draftOrder.DraftOrderItems.Where(x => x.TicketType == seat.TicketType);

                    foreach (var draftOrderItem in items)
                    {
                        // TODO: this is a hack - I need to check if I can do this using an ID or something
                        // I need to make sure that the item being added is the one being incremented
                        if (draftOrderItem.Ticket.ExtrasComparer == seat.TicketDetails.ExtrasComparer)
                        {
                            draftOrderItem.ReservedTickets = seat.Quantity;
                        }
                    }

                    var poItems = pricedOrder.PricedOrderLines.Where(x => x.Name == seat.TicketDetails.Name);
                    
                    foreach (var pricedOrderLine in poItems)
                    {
                        pricedOrderLine.Quantity = seat.Quantity;
                    }
                }

                draftOrder.State = state;
                draftOrder.ReservationExpirationDate = reservationExpiration;
                draftOrder.OrderVersion = orderVersion;

                await _reservationDbContext.SaveChangesAsync();
            }
        }
    }
}