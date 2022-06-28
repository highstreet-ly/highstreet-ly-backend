using System;
using System.Threading.Tasks;
using Baseline;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Payments.Contracts;
using Highstreetly.Payments.Resources;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Charge = Stripe.Charge;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;

namespace Highstreetly.Payments.Api.Controllers
{
    [Route("api/v1/webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IBusClient _busClient;
        private readonly string _secret;
        private readonly IAzureStorage _azureStorage;
        private PaymentsDbContext _paymentsDbContext;

        public WebhookController(
            ILoggerFactory loggerFactory,
            IBusClient busClient,
            StripeConfiguration stripeConfiguration,
            IAzureStorage azureStorage,
            PaymentsDbContext paymentsDbContext)
        {
            _logger = loggerFactory.CreateLogger<WebhookController>();
            _busClient = busClient;
            _azureStorage = azureStorage;
            _paymentsDbContext = paymentsDbContext;
            _secret = stripeConfiguration.WebHookSecret;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            _logger.LogInformation("POSTING into stripe webhook");

            try
            {
                var json = await HttpContext.Request.Body.ReadAllTextAsync();
                Request.Headers.TryGetValue("Stripe-Signature", out var stripeSig);

                var stripeEvent =
                    EventUtility
                        .ConstructEvent(json, stripeSig, _secret, throwOnApiVersionMismatch: false);

                var id = NewId.NextGuid();
                HsStripeEvent hsStripeEvent = new HsStripeEvent
                {
                    Id = NewId.NextGuid()
                };

                string blobName = string.Empty;

                switch (stripeEvent.Type)
                {
                    case "application_fee.refund.updated":
                        blobName =
                            $"{((ApplicationFee) stripeEvent.Data.Object).Id}-application_fee.refund.updated-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                               blobName,
                                JsonConvert.SerializeObject((ApplicationFee)stripeEvent.Data.Object));

                        await _busClient.Publish<IApplicationFeeRefundUpdated>(new ApplicationFeeRefundUpdated
                        {
                            HsEventId = blobName
                        });
                        break;
                    case "application_fee.refunded":
                        blobName =
                            $"{((ApplicationFee) stripeEvent.Data.Object).Id}-application_fee.refunded-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((ApplicationFee)stripeEvent.Data.Object));

                        await _busClient.Publish<IApplicationFeeRefunded>(new ApplicationFeeRefunded
                        {
                            HsEventId = blobName
                        });
                        break;
                    case "application_fee.created":
                        blobName =
                            $"{((ApplicationFee) stripeEvent.Data.Object).Id}-application_fee.created-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((ApplicationFee)stripeEvent.Data.Object));

                        await _busClient.Publish<IApplicationFeeCreated>(new ApplicationFeeCreated
                        {
                            HsEventId = blobName
                        });
                        break;

                    case "charge.refund.updated":
                        blobName = $"{((Charge) stripeEvent.Data.Object).Id}-charge.refund.updated-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((Charge)stripeEvent.Data.Object));

                        await _busClient.Publish<IChargeRefundUpdated>(new ChargeRefundUpdated
                        {
                            HsEventId = blobName
                        });
                        break;
                    case "charge.refunded":
                        blobName = $"{((Charge) stripeEvent.Data.Object).Id}-charge.refunded-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((Charge)stripeEvent.Data.Object));

                        var charge = ((Charge)stripeEvent.Data.Object);

                        var orderId = charge.Metadata["sona-order-id"];

                        await _busClient.Publish<IChargeRefunded>(new ChargeRefunded
                        {
                            HsEventId = blobName,
                            OrderId = Guid.Parse(orderId),
                            ReceiptUrl = charge.ReceiptUrl
                        });
                        break;
                    case "charge.succeeded":
                        blobName = $"{((Charge) stripeEvent.Data.Object).Id}-charge.succeeded-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((Charge)stripeEvent.Data.Object));

                        await _busClient.Publish<IChargeSucceeded>(new ChargeSucceeded
                        {
                            HsEventId = blobName
                        });
                        break;
                    case "payment_intent.canceled":
                        
                        // TODO: fire ICancelThirdPartyProcessorPayment 
                        
                        // await _azureStorage
                        //     .UploadJsonPayloadToAuzureBlob(
                        //         $"{((PaymentIntent)stripeEvent.Data.Object).Id}-payment_intent.processing",
                        //         JsonConvert.SerializeObject((PaymentIntent)stripeEvent.Data.Object));
                        //
                        // await _busClient.Publish<IPaymentIntentProcessing>(new PaymentIntentProcessing
                        //                                                    {
                        //                                                        HsEventId = $"{((PaymentIntent)stripeEvent.Data.Object).Id}-payment_intent.processing"
                        //                                                    });
                        //
                        // hsStripeEvent.Data = JsonConvert.SerializeObject(new PaymentIntentData
                        //                                                  {
                        //                                                      BlobId = id.ToString(),
                        //                                                      PaymentIntentId = stripeEvent.Data.RawObject.id
                        //                                                  });

                        break;
                    case "payment_intent.processing":
                        blobName =
                            $"{((PaymentIntent) stripeEvent.Data.Object).Id}-payment_intent.processing-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((PaymentIntent)stripeEvent.Data.Object));

                        await _busClient.Publish<IPaymentIntentProcessing>(new PaymentIntentProcessing
                        {
                            HsEventId = blobName
                        });

                        hsStripeEvent.Data = JsonConvert.SerializeObject(new PaymentIntentData
                        {
                            BlobId = id.ToString(),
                            PaymentIntentId = stripeEvent.Data.RawObject.id
                        });
                        break;
                    case "payment_intent.payment_failed":
                        blobName =
                            $"{((PaymentIntent) stripeEvent.Data.Object).Id}-payment_intent.payment_failed-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((PaymentIntent)stripeEvent.Data.Object));

                        await _busClient.Publish<IPaymentIntentFailed>(new PaymentIntentFailed
                        {
                            HsEventId = blobName
                        });

                        hsStripeEvent.Data = JsonConvert.SerializeObject(new PaymentIntentData
                        {
                            BlobId = ((PaymentIntent)stripeEvent.Data.Object).Id,
                            PaymentIntentId = stripeEvent.Data.RawObject.id
                        });
                        break;
                    case "payment_intent.succeeded":
                        blobName =
                            $"{((PaymentIntent) stripeEvent.Data.Object).Id}-payment_intent.succeeded-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((PaymentIntent)stripeEvent.Data.Object));

                        await _busClient.Publish<IPaymentIntentSucceeded>(new PaymentIntentSucceeded
                        {
                            HsEventId = blobName
                        });

                        hsStripeEvent.Data = JsonConvert.SerializeObject(new PaymentIntentData
                        {
                            BlobId = ((PaymentIntent)stripeEvent.Data.Object).Id,
                            PaymentIntentId = stripeEvent.Data.RawObject.id
                        });
                        break;
                    case "payment_intent.created":
                        blobName =
                            $"{((PaymentIntent) stripeEvent.Data.Object).Id}-payment_intent.created-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((PaymentIntent)stripeEvent.Data.Object));

                        await _busClient.Publish<IPaymentIntentCreated>(new PaymentIntentCreated
                        {
                            HsEventId = blobName
                        });

                        hsStripeEvent.Data = JsonConvert.SerializeObject(new PaymentIntentData
                        {
                            BlobId = ((PaymentIntent)stripeEvent.Data.Object).Id,
                            PaymentIntentId = stripeEvent.Data.RawObject.id
                        });
                        break;
                    case "transfer.created":
                        blobName = $"{((Transfer) stripeEvent.Data.Object).Id}-transfer.created-{Guid.NewGuid()}";
                        await _azureStorage
                            .UploadJsonPayloadToAuzureBlob(
                                blobName,
                                JsonConvert.SerializeObject((Transfer)stripeEvent.Data.Object));

                        await _busClient.Publish<ITransferCreated>(new TransferCreated
                        {
                            HsEventId = blobName
                        });

                        break;
                    default:
                        // Handle other event types
                        _logger.LogInformation($"Unhandled stripe event: {stripeEvent.Type}");
                        _logger.LogInformation(JsonConvert.SerializeObject(stripeEvent));
                        break;
                }

                _paymentsDbContext.HsStripeEvents.Add(hsStripeEvent);
                await _paymentsDbContext.SaveChangesAsync();

                return new EmptyResult();

            }
            catch (StripeException e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}