using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeRefundedHandler : IConsumer<IChargeRefunded>
    {
        private readonly ILogger<ChargeRefundedHandler> _logger;
        private readonly IAzureStorage _azureStorage;
        private PaymentsDbContext _paymentsDbContext;

        public ChargeRefundedHandler(
            ILogger<ChargeRefundedHandler> logger,
            IAzureStorage azureStorage, PaymentsDbContext paymentsDbContext)
        {
            _logger = logger;
            _azureStorage = azureStorage;
            _paymentsDbContext = paymentsDbContext;
        }

        public async Task Consume(
            ConsumeContext<IChargeRefunded> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var charge = JsonConvert.DeserializeObject<Charge>(stripeEvent);

                var c = _paymentsDbContext.Charges.First(x => x.ChargeId == charge.Id);

                c.Refunded = true;

                await _paymentsDbContext.SaveChangesAsync();

                _logger.LogInformation($"Succeeded fetching intent from payload: {charge.Id}");
            }
        }
    }
}