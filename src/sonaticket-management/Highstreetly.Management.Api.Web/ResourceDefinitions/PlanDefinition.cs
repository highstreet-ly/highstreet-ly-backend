using System;
using Highstreetly.Infrastructure.Messaging;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Resources;
using Plan = Highstreetly.Management.Resources.Plan;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;

namespace Highstreetly.Management.Api.Web.ResourceDefinitions
{
    /// <summary>
    /// When a plan is created we need to send it to stripe as a
    /// product and price that can be subscribed to
    /// TODO: move this to a handler
    /// NOTE: this is on hold now - user product subs needs more thought
    /// </summary>
    public class PlanDefinition : JsonApiResourceDefinition<Plan, Guid>
    {
        private readonly IBusClient _busClient;
        private readonly StripeConfiguration _stripeConfiguration;
        private ManagementDbContext _managementDbContext;

        public PlanDefinition(
            IResourceGraph resourceGraph, 
            IBusClient busClient,
            StripeConfiguration stripeConfiguration, 
            ManagementDbContext managementDbContext) : base(resourceGraph)
        {
            _busClient = busClient;
            _stripeConfiguration = stripeConfiguration;
            _managementDbContext = managementDbContext;
        }

        // public override async Task OnWriteSucceededAsync(Plan resource, OperationKind operationKind, CancellationToken cancellationToken)
        // {
        //     await base.OnWriteSucceededAsync(resource, operationKind, cancellationToken);
        //     Stripe.StripeConfiguration.ApiKey = _stripeConfiguration.ApiKey;
        //
        //     var productCreateOptions = new ProductCreateOptions
        //     {
        //         Name = resource.Name,
        //         Metadata = new() {{"highstreetly-plan-id",resource.Id.ToString()}}
        //     };
        //     
        //     var productService = new ProductService();
        //     var product = await productService.CreateAsync(productCreateOptions, cancellationToken: cancellationToken);
        //
        //     var priceService = new PriceService();
        //
        //     var priceCreateMonthlyOptions = new PriceCreateOptions
        //     {
        //         Product = product.Id,
        //         UnitAmount = resource.Price,
        //         Currency = "gbp",
        //         
        //         Recurring = new PriceRecurringOptions()
        //         {
        //             Interval = resource.PeriodUnit,
        //             IntervalCount = resource.Period,
        //         },
        //         Metadata = new() { { "highstreetly-plan-id", resource.Id.ToString() } }
        //     };
        //
        //     var price = await priceService.CreateAsync(priceCreateMonthlyOptions, cancellationToken: cancellationToken);
        //     var md = resource.Metadata;
        //
        //     md.Add("stripe-product-id", product.Id);
        //     md.Add("stripe-price-id", price.Id);
        //
        //     resource.Metadata = md;
        //
        //     await _managementDbContext.SaveChangesAsync(cancellationToken);
        // }
    }
}