using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentProcessingHandler : IConsumer<IPaymentIntentProcessing>
    {
        private readonly ILogger<PaymentIntentProcessingHandler> _logger;
        private readonly PaymentsDbContext _documentSession;
        private IAzureStorage _azureStorage;

        public PaymentIntentProcessingHandler(
            ILogger<PaymentIntentProcessingHandler> logger,
            PaymentsDbContext documentSession,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            _documentSession = documentSession;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IPaymentIntentProcessing> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var paymentIntent = JsonConvert.DeserializeObject<PaymentIntent>(stripeEvent);

                var correlation = Guid.Parse(paymentIntent.Metadata["sona-correlation-id"]);

                _logger.LogInformation($"Succeeded fetching intent from payload: {paymentIntent.Id}");
            }
        }
    }
}