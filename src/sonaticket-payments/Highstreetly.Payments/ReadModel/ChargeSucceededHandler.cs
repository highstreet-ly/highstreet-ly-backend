using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeSucceededHandler : IConsumer<IChargeSucceeded>
    {
        private readonly ILogger<ChargeSucceededHandler> _logger;
        private IAzureStorage _axAzureStorage;
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly IMapper _mapper;
        private readonly IEventOrganiserSiglnalrService _eventOrganiserSiglnalrService;

        public ChargeSucceededHandler(
            ILogger<ChargeSucceededHandler> logger,

            IMapper mapper,
            IEventOrganiserSiglnalrService eventOrganiserSiglnalrService,
            PaymentsDbContext paymentsDbContext,
            IAzureStorage axAzureStorage)
        {
            _logger = logger;

            _mapper = mapper;
            _eventOrganiserSiglnalrService = eventOrganiserSiglnalrService;
            _paymentsDbContext = paymentsDbContext;
            _axAzureStorage = axAzureStorage;
        }

        public async Task Consume(
            ConsumeContext<IChargeSucceeded> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var stripeEvent = await _axAzureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);
                var chargeResponse = JsonConvert.DeserializeObject<Charge>(stripeEvent);

                if (chargeResponse == null)
                {
                    _logger.LogInformation($"Failed fetching charge from payload: {context.Message.HsEventId}");
                    throw new Exception($"Failed fetching charge from payload: {context.Message.HsEventId}");
                }

                _logger.LogInformation($"Succeeded fetching intent from payload: {chargeResponse.Id}");

                var charge = new Resources.Charge
                {
                    ChargeId = chargeResponse.Id,
                    PaymentIntent = chargeResponse.PaymentIntent,
                    Amount = chargeResponse.Amount ?? 0,
                    Application = chargeResponse.Application,
                    Currency = chargeResponse.Currency,
                    Description = chargeResponse.Description,
                    Refunded = chargeResponse.Refunded,
                    AmountCaptured = chargeResponse.AmountCaptured ?? 0,
                    AmountRefunded =  chargeResponse.AmountRefunded ?? 0,
                    ApplicationFeeAmount = chargeResponse.ApplicationFeeAmount ?? 0,
                    FailureCode = chargeResponse.FailureCode,
                    FailureMessage = chargeResponse.FailureMessage,
                    RecieptUrl = chargeResponse.ReceiptUrl,
                    DateCreated = DateTime.UtcNow
                };

                var payment =
                    _paymentsDbContext.Payments.FirstOrDefault(x => x.PaymentIntentId == charge.PaymentIntent);

                if (payment == null)
                {
                    _logger.LogInformation($"Failed fetching payment for intent: {charge.PaymentIntent}");
                    throw new Exception($"Failed fetching payment for intent: {charge.PaymentIntent}");
                }

                charge.PaymentId = payment.Id;

                _paymentsDbContext.Charges.Add(charge);

                await _paymentsDbContext.SaveChangesAsync();

                await _eventOrganiserSiglnalrService.Send(
                    chargeResponse.Metadata.SonaOrgId,
                    JsonConvert.SerializeObject(new
                    {
                        Status = SignalrConstants.PaymentProcessed,
                        OrderId = chargeResponse.Metadata.OrderId
                    }));
            }
        }
    }
}