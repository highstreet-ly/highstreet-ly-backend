using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIOrderConfirmed : IConsumer<IOrderConfirmed>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        public RegistrationProcessManagerRouterIOrderConfirmed(Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public Task Consume(ConsumeContext<IOrderConfirmed> @event)
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
                Trace.TraceInformation("Failed to locate the registration process manager to complete with id {0}.",
                    @event.Message.SourceId);
            }

            return Task.CompletedTask;
        }
    }
}