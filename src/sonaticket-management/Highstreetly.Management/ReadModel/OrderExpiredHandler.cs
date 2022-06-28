using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Management.ReadModel
{
    public class OrderExpiredHandler : IConsumer<IOrderExpired>
    {
        private readonly IJsonApiClient<DraftOrder, Guid> _draftOrderClient;
        private readonly ILogger<OrderExpiredHandler> _logger;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly ManagementDbContext _managementDbContext;

        public OrderExpiredHandler(IJsonApiClient<DraftOrder, Guid> draftOrderClient, ILogger<OrderExpiredHandler> logger, IEventOrganiserSiglnalrService eventOrganiserSiglnalrService, ManagementDbContext managementDbContext)
        {
            _draftOrderClient = draftOrderClient;
            _logger = logger;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _managementDbContext = managementDbContext;
        }


        public async Task Consume(
            ConsumeContext<IOrderExpired> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation("Consuming IOrderExpired in OrderExpiredHandler");

                var order = await _draftOrderClient.GetAsync(context.Message.SourceId);

                var eventInstance = await _managementDbContext
                    .EventInstances
                    .Include(x => x.EventSeries)
                    .ThenInclude(x => x.EventOrganiser)
                    .Where(x => x.Id == order.EventInstanceId)
                    .FirstAsync();

                var stats = _managementDbContext.DashboardStats.First(x =>
                    x.EventOrganiserId == eventInstance.EventSeries.EventOrganiser.Id);
                stats.TotalOrdersAbandoned += 1;

                await _managementDbContext.SaveChangesAsync();

                await _eventOrganiserSiglnalrService.Send(eventInstance.EventSeries.EventOrganiserId.ToString(),
                    JsonConvert.SerializeObject(new
                    {
                        Status = SignalrConstants.OrderExpired,
                        OrderId = order.Id,
                        EventInstanceId = eventInstance.Id
                    }));
            }
        }
    }
}