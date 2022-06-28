using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Permissions.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentSucceededHandler : IConsumer<IPaymentIntentSucceeded>
    {
        private readonly ILogger<PaymentIntentSucceededHandler> _logger;
       
        private readonly IJsonApiClient<User, Guid> _userClient;
        private readonly IBusClient _busClient;
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly IAzureStorage _azureStorage;

        public PaymentIntentSucceededHandler(
            ILogger<PaymentIntentSucceededHandler> logger,
          
            IJsonApiClient<User, Guid> userClient,
            IBusClient busClient, PaymentsDbContext paymentsDbContext,
            IAzureStorage azureStorage)
        {
            _logger = logger;
            _userClient = userClient;
            _busClient = busClient;
            _paymentsDbContext = paymentsDbContext;
            _azureStorage = azureStorage;
        }

        public async Task Consume(
            ConsumeContext<IPaymentIntentSucceeded> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

                var paymentIntent = JsonConvert.DeserializeObject<PaymentIntent>(stripeEvent);

                if (paymentIntent == null)
                {
                    _logger.LogInformation($"Failed fetching payment intent from payload: {context.Message.HsEventId}");
                    throw new Exception($"Failed fetching payment intent from payload: {context.Message.HsEventId}");
                }

                var correlation = Guid.Parse(paymentIntent.Metadata["sona-correlation-id"]);

                _logger.LogInformation($"Succeeded fetching intent from payload: {paymentIntent.Id}");

                // Fulfill the customer's purchase
                var payment = _paymentsDbContext.Payments.FirstOrDefault(x => x.PaymentIntentId == paymentIntent.Id);

                if (payment == null)
                {
                    _logger.LogInformation($"Failed fetching payment for intent: {paymentIntent.Id}");
                    throw new Exception($"Failed fetching payment for intent: {paymentIntent.Id}");
                }

                _logger.LogInformation($"Looking for user with email {payment.Email}");

                var queryBuilder = new QueryBuilder()
                    .Equalz(
                        "normalized-email",
                        payment.Email.ToUpper())
                    .Includes("claims");

                var users = await _userClient.GetListAsync(queryBuilder);

                var user = users.FirstOrDefault();

                await _busClient.Send<ICompleteThirdPartyProcessorPayment>(new CompleteThirdPartyProcessorPayment
                {
                    PaymentIntentId = paymentIntent.Id,
                    PaymentId = payment.Id,
                    Amount = paymentIntent.Amount,
                    ApplicationFeeAmount = 1000,
                    Currency = "gbp",
                    //Last4 = intent.Source.Last4,
                    Created = paymentIntent.Created,
                    OrderId = payment.OrderId,
                    Email = payment.Email,
                    UserId = user.Id,
                    CorrelationId = correlation
                });
            }
        }
    }
}