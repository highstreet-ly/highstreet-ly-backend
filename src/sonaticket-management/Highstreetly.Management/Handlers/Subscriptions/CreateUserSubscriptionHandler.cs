using ChargeBee.Api;
using ChargeBee.Models.Enums;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Resources;
using Highstreetly.Permissions.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Microsoft.Extensions.Configuration;


namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreateUserSubscriptionHandler : IConsumer<ICreateUserSubscription>
    {
        private readonly ILogger<CreateUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;
        private readonly IJsonApiClient<User, Guid> _userClient;

        public CreateUserSubscriptionHandler(
            ILogger<CreateUserSubscriptionHandler> logger,
            ManagementDbContext managementDbContext,
            IJsonApiClient<User, Guid> userClient, 
            IConfiguration config)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
            _userClient = userClient;
            ApiConfig.Configure(config["ChargeBeeSite"], config["ChargeBeeKey"]);
        }

        public async Task Consume(ConsumeContext<ICreateUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            // we need to know this is a user we manage
            // - if not we need to cancel the subscription and email them to say they used the wrong email

            var queryBuilder = new QueryBuilder()
                .Equalz(
                    "normalized-email",
                    context.Message.SubscriptionCreate.Content.Customer.Email.ToUpper());

            var usersByEmail = await _userClient.GetListAsync(queryBuilder);

            if (!usersByEmail.Any())
            {
                _logger.LogInformation("No user found - throwing so that we can retry");
                throw new Exception(
                    $"We don't have a user with this email - throwing so that we can retry a few times");
            }

            var user = usersByEmail.First();

            // an organisation can only belong to one subscription so delete any other ones they have at CB
            var contentSubscription = context.Message.SubscriptionCreate.Content.Subscription;

            var existingSubscriptions = _managementDbContext
                .Subscriptions
                .Where(x => x.EventOrganiserId == user.CurrentEoid &&
                            x.Status != ChargeBee.Models.Subscription.StatusEnum.Cancelled.ToString());

            foreach (var existingSubscription in existingSubscriptions)
            {
                try
                {
                    if (existingSubscription.IntegrationId != contentSubscription.Id)
                    {
                        _logger.LogInformation($"Cancelling the users other subscriptions: {existingSubscription.IntegrationId}");
                        await ChargeBee.Models.Subscription
                            .Cancel(existingSubscription.IntegrationId)
                            .CreditOptionForCurrentTermCharges(CreditOptionForCurrentTermChargesEnum.Prorate)
                            .EndOfTerm(false)
                            .RequestAsync();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                }
            }

            var plan = _managementDbContext.Plans.Single(x =>
                x.IntegrationId == contentSubscription.PlanId);

            var newSub = new Subscription
            {
                BillingPeriod = contentSubscription.BillingPeriod,
                BillingPeriodUnit = contentSubscription.BillingPeriodUnit,
                CurrencyCode = contentSubscription.CurrencyCode,
                CurrentTermEnd = contentSubscription.CurrentTermEnd,
                CurrentTermStart = contentSubscription.CurrentTermStart,
                CustomerId = contentSubscription.CustomerId,
                UserId = user.Id,
                UserEmail = user.Email,
                CreatedAt = contentSubscription.CreatedAt,
                DueInvoicesCount = contentSubscription.DueInvoicesCount,
                IntegrationId = contentSubscription.Id,
                PlanFreeQuantity = contentSubscription.PlanFreeQuantity,
                PlanUnitPrice = contentSubscription.PlanUnitPrice,
                PlanQuantity = contentSubscription.PlanQuantity,
                StartedAt = contentSubscription.StartedAt,
                Status = contentSubscription.Status,
                TrialEnd = contentSubscription.TrialEnd,
                TrialStart = contentSubscription.TrialStart,
                PlanIntegrationId = contentSubscription.PlanId,
                PlanId = plan.Id,
                EventOrganiserId = user.CurrentEoid
            };

            await _managementDbContext.Subscriptions.AddAsync(newSub);

            if (contentSubscription.Addons != null)
            {
                var addOnIds = contentSubscription.Addons.Select(x => x.Id);

                var addOns = _managementDbContext.AddOns.Where(x => addOnIds.Contains(x.IntegrationId));

                await _managementDbContext.SubscriptionAddOns.AddRangeAsync(addOns.Select(x => new SubscriptionAddOn
                {
                    AddOn = x,
                    Subscription = newSub
                }));
            }

            await _managementDbContext.SaveChangesAsync();
        }
    }
}
