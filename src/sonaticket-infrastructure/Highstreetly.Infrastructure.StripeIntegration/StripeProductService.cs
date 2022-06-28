using System;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Stripe;

namespace Highstreetly.Infrastructure.StripeIntegration
{
    public class StripeProductService : IStripeProductService
    {
        private readonly IJsonApiClient<TicketTypeConfiguration, Guid> _ticketTypeConfigurationApiClient;

        public StripeProductService(Configuration.StripeConfiguration stripeConfiguration, IJsonApiClient<TicketTypeConfiguration, Guid> ticketTypeConfigurationApiClient)
        {
            _ticketTypeConfigurationApiClient = ticketTypeConfigurationApiClient;
            StripeConfiguration.ApiKey = stripeConfiguration.ApiKey;
        }

        public async Task EnsureProductExistsOnStripe(
            string productName, 
            Guid ticketTypeConfigId,
            long productPrice)
        {
            var ticketTypeConfig = await _ticketTypeConfigurationApiClient.GetAsync(ticketTypeConfigId);
            var productCreateOptions = new ProductCreateOptions
            {
                Name = $"{productName}-{ticketTypeConfigId}"
            };

            var productService = new ProductService();
            var product = await productService.CreateAsync(productCreateOptions);

            var priceCreateOptions = new PriceCreateOptions
            {
                Product = product.Id,
                UnitAmount = productPrice,
                Currency = "gbp"
            };

            var priceService = new PriceService();
            var price = await priceService.CreateAsync(priceCreateOptions);

            // var md = ticketTypeConfig.Metadata;
            //
            // md.Add("stripe-product-id", product.Id);
            // md.Add("stripe-price-id", price.Id);
            //
            // ticketTypeConfig.Metadata = md;
        }
    }
}