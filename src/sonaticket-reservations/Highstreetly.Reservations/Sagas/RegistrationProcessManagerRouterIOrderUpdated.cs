using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIOrderUpdated : IConsumer<IOrderUpdated>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;

        public RegistrationProcessManagerRouterIOrderUpdated(Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public Task Consume(ConsumeContext<IOrderUpdated> @event)
        {
            using var context = _contextFactory();
            var pm = context.Find(x => x.OrderId == @event.Message.SourceId);
            if (pm != null)
            {
                pm.Handle(@event.Message);

                context.Save(pm);
            }
            else
            {
                Trace.TraceError(
                    "Failed to locate the registration process manager handling the order with id {0}.",
                    @event.Message.SourceId);
            }


            return Task.CompletedTask;
        }
    }
}