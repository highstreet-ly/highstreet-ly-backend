using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Payments.ViewModels.Payments.PaymentModels.ApplicationFee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Payments.ReadModel
{
    public class ApplicationFeeRefundUpdatedHandler : IConsumer<IApplicationFeeRefundUpdated>
    {
        private readonly ILogger<ApplicationFeeRefundUpdatedHandler> _logger;
        private readonly IAzureStorage _azureStorage;

        public ApplicationFeeRefundUpdatedHandler(
            ILogger<ApplicationFeeRefundUpdatedHandler> logger,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IApplicationFeeRefundUpdated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var applicationFee = JsonConvert.DeserializeObject<ApplicationFee>(stripeEvent);

                _logger.LogInformation($"Succeeded fetching intent from payload: {applicationFee.Id}");
            }
        }
    }
}