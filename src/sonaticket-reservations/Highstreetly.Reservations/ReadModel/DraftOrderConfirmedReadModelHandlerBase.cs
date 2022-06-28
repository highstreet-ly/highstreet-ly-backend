using System;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Reservations.Contracts.Requests;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.ReadModel
{
  public abstract class DraftOrderConfirmedReadModelHandlerBase<T> : DraftOrderReadModelHandlerBase<T>
    {
        protected readonly ReservationDbContext ReservationDbContext;
        private RetryPolicy _waitForOrder;

        protected DraftOrderConfirmedReadModelHandlerBase(ReservationDbContext reservationDbContext)
        {
            ReservationDbContext = reservationDbContext;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2),
                                              TimeSpan.FromSeconds(3),
                                          });
        }

        protected async Task ConsumeOrderConfirmed(IOrderConfirmed @event)
        {

            var model = _waitForOrder.Execute(() =>  ReservationDbContext.DraftOrders.FirstOrDefault(x => x.Id == @event.SourceId));
            if (WasNotAlreadyHandled(model, @event.Version))
            {
                model.State = States.Confirmed;
                model.OrderVersion = @event.Version;
                await ReservationDbContext.SaveChangesAsync();
            }
        }
    }
}