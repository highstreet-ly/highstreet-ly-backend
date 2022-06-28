using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Resources;
using MassTransit;
using Stripe;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;

namespace Highstreetly.Payments.ReadModel
{
    public class DraftOrderCreatedHandler : IConsumer<IDraftOrderCreated>
    {
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        private readonly IJsonApiClient<EventOrganiser, Guid> _eventOrganiserClient;
        private readonly PaymentsDbContext _paymentsDbContext;

        public DraftOrderCreatedHandler(
            StripeConfiguration stripeConfiguration,
            IJsonApiClient<EventInstance, Guid> eventInstanceClient, 
            IJsonApiClient<EventOrganiser, Guid> eventOrganiserClient,
            PaymentsDbContext paymentsDbContext)
        {
            _eventInstanceClient = eventInstanceClient;
            _eventOrganiserClient = eventOrganiserClient;
            _paymentsDbContext = paymentsDbContext;
            Stripe.StripeConfiguration.ApiKey = stripeConfiguration.ApiKey;
        }

        public async Task Consume(ConsumeContext<IDraftOrderCreated> context)
        {
            var paymentIntents = new PaymentIntentService();
            
            var eventInstance = await _eventInstanceClient.GetAsync(context.Message.EventInstanceId);
            var organiser = await _eventOrganiserClient.GetAsync(eventInstance.EventOrganiserId, allowApiAuthIfNeeded: true);
            
            var createOptions = new PaymentIntentCreateOptions
                                {
                                    PaymentMethodTypes = new List<string>
                                                         {
                                                             "card",
                                                         },
                                    Amount = 30, // this needs to be _something_ (positive integer > 30) so default to 1 
                                    Currency = "gbp",
                                    ApplicationFeeAmount = 0,
                                    TransferData =
                                        new PaymentIntentTransferDataOptions
                                        {
                                            Destination =
                                                organiser.StripeAccountId
                                        },
                                    Metadata = new Dictionary<string, string>
                                               {
                                                   {"sona-correlation-id", context.CorrelationId.ToString()},
                                                   {"sona-order-id", context.Message.OrderId.ToString()},
                                                   {"sona-org-id", organiser.Id.ToString()}
                                               }
                                };

            var intent = await paymentIntents.CreateAsync(createOptions, cancellationToken: context.CancellationToken);

            var payment = new Payment()
                          {
                              OrderId = context.Message.OrderId,
                              PaymentIntentId = intent.Id,
                              Id = NewId.NextGuid(),
                              EventInstanceId = context.Message.EventInstanceId,
                              PaymentIntentSecret = intent.ClientSecret,
                          };

            await _paymentsDbContext.Payments.AddAsync(payment);
            await _paymentsDbContext.SaveChangesAsync();
        }
    }
}