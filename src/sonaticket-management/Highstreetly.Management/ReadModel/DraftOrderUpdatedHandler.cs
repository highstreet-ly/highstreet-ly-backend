using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Highstreetly.Management.ReadModel
{
    public class DraftOrderUpdatedHandler : IConsumer<IDraftOrderUpdated>
    {
        private readonly ILogger<AvailableTicketsChangedHandler> _logger;
        private readonly IJsonApiClient<DraftOrder, Guid> _draftOrderClient;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;
        private readonly ManagementDbContext _managementDbContext;
        private RetryPolicy _waitForOrder;

        public DraftOrderUpdatedHandler(
            ILogger<AvailableTicketsChangedHandler> logger,
            IJsonApiClient<DraftOrder, Guid> draftOrderClient,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _draftOrderClient = draftOrderClient;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _managementDbContext = managementDbContext;
            
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                               {
                                                   TimeSpan.FromSeconds(1),
                                                   TimeSpan.FromSeconds(2),
                                                   TimeSpan.FromSeconds(3),
                                               });
            
        }

        public async Task Consume(
            ConsumeContext<IDraftOrderUpdated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                _logger.LogInformation(
                    $"Consuming IOrderPlaced in OrderPlacedHandler for {context.Message.OrderId}");

                var draftOrder = await _draftOrderClient.GetAsync(context.Message.OrderId);

                var order = _waitForOrder.Execute(() => _managementDbContext
                                                        .Orders
                                                        .First(x => x.Id == context.Message.OrderId));

                order.IsToTable = draftOrder.IsToTable;
                order.TableInfo = draftOrder.TableInfo;
                order.IsLocalDelivery = draftOrder.IsLocalDelivery;
                order.IsClickAndCollect = draftOrder.IsClickAndCollect;
                order.IsNationalDelivery = draftOrder.IsNationalDelivery;
                order.MakeSubscription = draftOrder.MakeSubscription;

                await _managementDbContext.SaveChangesAsync(context.CancellationToken);

                var eventInstance =
                    _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == order.EventInstanceId);
                var series = _managementDbContext.EventSeries.FirstOrDefault(x => x.Id == eventInstance.EventSeriesId);
                var organiser =
                    _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == series.EventOrganiserId);

                await _eventOrganiserSiglnalrService.Send(organiser.Id.ToString(), JsonConvert.SerializeObject(new
                {
                    Status = SignalrConstants.OrderUpdated,
                    OrderId = order.Id,
                    EventInstanceId = eventInstance.Id
                }));
            }
        }
    }
}