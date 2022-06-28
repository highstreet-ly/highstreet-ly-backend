using System;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIExpireRegistrationProcess :
        IConsumer<IExpireRegistrationProcess>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;

        public RegistrationProcessManagerRouterIExpireRegistrationProcess(
            Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Task Consume(ConsumeContext<IExpireRegistrationProcess> command)
        {
            using var context = _contextFactory();
            var pm = context.Find(x => x.Id == command.Message.ProcessId);
            if (pm != null)
            {
                pm.Handle(command.Message);

                context.Save(pm);
            }

            return Task.CompletedTask;
        }
    }
}