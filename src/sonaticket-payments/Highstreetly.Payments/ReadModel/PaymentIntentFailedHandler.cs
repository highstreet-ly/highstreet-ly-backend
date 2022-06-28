using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Stripe;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentFailedHandler : IConsumer<IPaymentIntentFailed>
    {
        private readonly IAzureStorage _azureStorage;
        private readonly IBusClient _busClient;
        private readonly ILogger<PaymentIntentFailedHandler> _logger;
        private readonly PaymentsDbContext _paymentsDbContext;

        public PaymentIntentFailedHandler(
            IBusClient busClient,
            ILogger<PaymentIntentFailedHandler> logger,
            PaymentsDbContext paymentsDbContext,
            IAzureStorage azureStorage)
        {
            _busClient = busClient;
            _logger = logger;
            _paymentsDbContext = paymentsDbContext;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IPaymentIntentFailed> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var paymentIntent = JsonConvert.DeserializeObject<PaymentIntent>(stripeEvent);

                _logger.LogInformation($"Succeeded fetching intent from payload: {paymentIntent.Id}");

                var correlationFailure = Guid.Parse(paymentIntent.Metadata["sona-correlation-id"]);
                var failedPayment = _paymentsDbContext
                                    .Payments
                                    .Include(x=>x.Charges)
                                    .Single(x => x.PaymentIntentId == paymentIntent.Id);

                
                _logger.LogInformation($"Failure: {paymentIntent.Id}");

                foreach (var paymentIntentCharge in paymentIntent.Charges)
                {
                    var charge = failedPayment.Charges.FirstOrDefault(x => x.ChargeId == paymentIntentCharge.Id);
                    var description = paymentIntentCharge.FailureMessage;

                    if (paymentIntentCharge.Outcome != null)
                    {
                        description = $"{description} - {paymentIntentCharge.Outcome.SellerMessage}";
                    }

                    if (charge == null)
                    {
                        failedPayment.Charges.Add(new Resources.Charge()
                                                  {
                                                      Amount = paymentIntentCharge.Amount,
                                                      Currency = paymentIntentCharge.Currency,
                                                      Description = paymentIntentCharge.Description,
                                                      FailureCode = paymentIntentCharge.FailureCode,
                                                      FailureMessage = description,
                                                      DateCreated = DateTime.UtcNow
                                                  });
                    }
                    else
                    {
                        charge.FailureCode = paymentIntentCharge.FailureCode;
                        charge.FailureMessage = description;
                    }
                }

                await  _paymentsDbContext.SaveChangesAsync();
            }
        }
    }
}