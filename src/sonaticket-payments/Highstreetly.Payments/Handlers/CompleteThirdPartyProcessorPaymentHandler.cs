using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Domain;
using Highstreetly.Permissions.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Highstreetly.Management.Contracts.Requests.Order;
using Plan = Highstreetly.Management.Contracts.Requests.Plan;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;

namespace Highstreetly.Payments.Handlers
{
    public class CompleteThirdPartyProcessorPaymentHandler : IConsumer<ICompleteThirdPartyProcessorPayment>
    {
        private readonly Func<IDataContext<ThirdPartyProcessorPayment>> _contextFactory;
        private readonly ILogger<CompleteThirdPartyProcessorPaymentHandler> _logger;
        private readonly IJsonApiClient<Order, Guid> _orderApiClient;
        private readonly IJsonApiClient<Plan, Guid> _planApiClient;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        private readonly IJsonApiClient<User, Guid> _userJsonApiClient;
        private readonly StripeConfiguration _stripeConfiguration;
        private readonly IIdentityService _identityService;
        private readonly IStripeUserService _stripeUserService;

        public CompleteThirdPartyProcessorPaymentHandler(
            Func<IDataContext<ThirdPartyProcessorPayment>> contextFactory,
            ILogger<CompleteThirdPartyProcessorPaymentHandler> logger,
            IJsonApiClient<Order, Guid> orderApiClient,
            IJsonApiClient<EventInstance, Guid> eventInstanceClient,
            IJsonApiClient<Plan, Guid> planApiClient,
            IJsonApiClient<User, Guid> userJsonApiClient,
            StripeConfiguration stripeConfiguration,
            IIdentityService identityService,
            IStripeUserService stripeUserService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _orderApiClient = orderApiClient;
            _eventInstanceClient = eventInstanceClient;
            _planApiClient = planApiClient;
            _userJsonApiClient = userJsonApiClient;
            _stripeConfiguration = stripeConfiguration;
            _identityService = identityService;
            _stripeUserService = stripeUserService;
        }

        public async Task Consume(
            ConsumeContext<ICompleteThirdPartyProcessorPayment> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                // Stripe.StripeConfiguration.ApiKey = _stripeConfiguration.ApiKey;
                var repository = _contextFactory();

                var payment = repository.Find(command.Message.PaymentId);

                if (payment != null)
                {
                    payment.Complete(command.Message.Email, command.Message.UserId);

                    repository.Save(payment);

                    var order = await _orderApiClient.GetAsync(payment.PaymentSourceId);
                    var ei = await _eventInstanceClient.GetAsync(order.EventInstanceId);

                    await command.Publish<ILinkCustomerAccountToStripe>(new LinkCustomerAccountToStripe
                    {
                        EventOrganiserId = ei.EventOrganiserId,
                        CustomerId = command.Message.UserId,
                        PaymentIntentId = command.Message.PaymentIntentId
                    });

                    // We can't do this until we start verifying users emails
                    // if (order.MakeSubscription)
                    // {
                    //     // create the product the user will subscribe to - by POSTing to 
                    //     // `/plans` on the management BC - this will also create the price
                    //     var plan = await _planApiClient.CreateAsync(new Plan
                    //     {
                    //         Name = $"customer-plan-{command.Message.UserId}-{DateTime.UtcNow.ToFileTimeUtc()}",
                    //         Price = (int)payment.TotalAmount,
                    //
                    //     });
                    //
                    //     var user = await _userJsonApiClient.GetAsync(command.Message.UserId, includes: new[] { "claims" });
                    //
                    //     var customerId = await _stripeUserService.EnsureUserExistsOnStripe(user);
                    //
                    //     var options = new SubscriptionCreateOptions
                    //     {
                    //         Customer = customerId,
                    //         Items = new List<SubscriptionItemOptions>
                    //     {
                    //         new SubscriptionItemOptions
                    //         {
                    //             Price = plan.Metadata["stripe-price-id"],
                    //         },
                    //     },
                    //     };
                    //
                    //     var subscriptionService = new SubscriptionService();
                    //     await subscriptionService.CreateAsync(options);
                    // }
                }
                else
                {
                    _logger.LogError(
                        "Failed to locate the payment entity with id {0} for the completed third party payment.",
                        command.Message.PaymentId);
                    throw new CompleteThirdPartyProcessorPaymentHandlerFailed();
                }
            }
        }
    }
    
    public class CompleteThirdPartyProcessorPaymentHandlerFailed : Exception{}
}