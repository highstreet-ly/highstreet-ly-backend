using System;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Processors;
using Highstreetly.Management.Contracts.Requests;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegistrationProcessManagerRouterIOrderPlaced : IConsumer<IOrderPlaced>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
         
        public RegistrationProcessManagerRouterIOrderPlaced(Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory, IJsonApiClient<EventInstance, Guid> eventInstanceClient)
        {
            _contextFactory = contextFactory;
            _eventInstanceClient = eventInstanceClient;
        }
        
        public async Task Consume(ConsumeContext<IOrderPlaced> @event)
        {
            var ei = await _eventInstanceClient.GetAsync(@event.Message.EventInstanceId);
            
            using var context = _contextFactory();
            var pm = context.Find(x => x.OrderId == @event.Message.SourceId) ?? new RegistrationProcessManager();

            pm.IsStockManaged = ei.IsStockManaged;
            
            pm.Handle(@event.Message);
            context.Save(pm);
        }
    }
}