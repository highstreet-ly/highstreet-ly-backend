using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeRefundUpdatedHandler : IConsumer<IChargeRefundUpdated>
    {
        private readonly ILogger<ChargeRefundUpdatedHandler> _logger;
        private readonly IAzureStorage _azureStorage;

        public ChargeRefundUpdatedHandler(
            ILogger<ChargeRefundUpdatedHandler> logger,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IChargeRefundUpdated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var charge = JsonConvert.DeserializeObject<Charge>(stripeEvent);

                _logger.LogInformation($"Succeeded fetching intent from payload: {charge.Id}");
            }
        }
    }
}