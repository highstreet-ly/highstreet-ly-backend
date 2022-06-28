using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApplicationFee = Highstreetly.Payments.ViewModels.Payments.PaymentModels.ApplicationFee.ApplicationFee;

namespace Highstreetly.Payments.ReadModel
{
    public class ApplicationFeeCreatedHandler : IConsumer<IApplicationFeeCreated>
    {
        private readonly ILogger<ApplicationFeeCreatedHandler> _logger;
        private readonly IAzureStorage _azureStorage;

        public ApplicationFeeCreatedHandler(
            ILogger<ApplicationFeeCreatedHandler> logger,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IApplicationFeeCreated> context)
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