using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterICommitOrder :
        IConsumer<ICommitOrder>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;

        public RegistrationProcessManagerRouterICommitOrder(
            Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Task Consume(ConsumeContext<ICommitOrder> command)
        {
            try
            {
                using var context = _contextFactory();
                var pm = context.Find(x => x.OrderId == command.Message.SourceId);
                if (pm != null)
                {
                    pm.Handle(command.Message);

                    context.Save(pm);
                }
                else
                {
                    Trace.TraceError(
                        "Failed to locate the registration process manager handling the order with id {0}.",
                        command.Message.SourceId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}