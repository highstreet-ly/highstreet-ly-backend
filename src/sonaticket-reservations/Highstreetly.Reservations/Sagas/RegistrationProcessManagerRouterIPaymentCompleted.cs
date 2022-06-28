using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIPaymentCompleted : IConsumer<IPaymentCompleted>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        public RegistrationProcessManagerRouterIPaymentCompleted(Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public Task Consume(ConsumeContext<IPaymentCompleted> @event)
        {
            using var context = _contextFactory();
            // TODO: should not skip the completed processes and try to re-acquire the reservation,
            // and if not possible due to not enough seats, move them to a "manual intervention" state.
            // This was not implemented but would be very important.
            var pm = context.Find(x => x.OrderId == @event.Message.PaymentSourceId);
            if (pm != null)
            {
                pm.Handle(@event.Message);

                context.Save(pm);
            }
            else
            {
                Trace.TraceError(
                    "Failed to locate the registration process manager handling the paid order with id {0}.",
                    @event.Message.PaymentSourceId);
            }

            return Task.CompletedTask;
        }
    }
}