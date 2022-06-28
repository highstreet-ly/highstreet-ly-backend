using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Reservations.Contracts.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Stripe;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;

namespace Highstreetly.Payments.ReadModel
{
    public class OrderTotalsCalculatedHandler : IConsumer<IOrderTotalsCalculated>
    {
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly AsyncRetryPolicy _waitForOrderToBePriced;
        private readonly ILogger<OrderTotalsCalculatedHandler> _logger;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        private readonly IJsonApiClient<EventOrganiser, Guid> _eventOrganiserClient;

        public OrderTotalsCalculatedHandler( StripeConfiguration stripeConfiguration, PaymentsDbContext paymentsDbContext, IJsonApiClient<PricedOrder, Guid> pricedOrderClient, IJsonApiClient<EventInstance, Guid> eventInstanceClient, IJsonApiClient<EventOrganiser, Guid> eventOrganiserClient, ILogger<OrderTotalsCalculatedHandler> logger)
        {
            _paymentsDbContext = paymentsDbContext;
            _pricedOrderClient = pricedOrderClient;
            _eventInstanceClient = eventInstanceClient;
            _eventOrganiserClient = eventOrganiserClient;
            _logger = logger;
            Stripe.StripeConfiguration.ApiKey = stripeConfiguration.ApiKey;
            _waitForOrderToBePriced = Policy
                                      .Handle<OrderNotPricedException>()
                                      .WaitAndRetryAsync(new[]
                                                         {
                                                             TimeSpan.FromSeconds(1),
                                                             TimeSpan.FromSeconds(2),
                                                             TimeSpan.FromSeconds(3),
                                                         });
        }
        
        public async Task Consume(ConsumeContext<IOrderTotalsCalculated> context)
        {
            _logger.LogInformation($"Running {nameof(OrderTotalsCalculatedHandler)} with order ID {context.Message.SourceId}");
            
            var paymentIntents = new PaymentIntentService();
            
            var pricedOrder = await _waitForOrderToBePriced.ExecuteAsync(() => GetPricedOrderAsync(context.Message.SourceId.ToString()));
            var eventInstance = await _eventInstanceClient.GetAsync(context.Message.EventInstanceId);
            var organiser = await _eventOrganiserClient.GetAsync(eventInstance.EventOrganiserId, allowApiAuthIfNeeded: true);

            var payment = _paymentsDbContext.Payments.First(x => x.OrderId == context.Message.SourceId);
            
            var customerPays = pricedOrder.Total;
            var operatorPays = organiser.PlatformFee ?? 0;

            await paymentIntents.UpdateAsync(payment.PaymentIntentId, new PaymentIntentUpdateOptions
                                                                      {
                                                                          Amount = customerPays ?? 0,
                                                                          ApplicationFeeAmount = operatorPays,
                                                                         
                                                                      });
        }
        
        private async Task<PricedOrder> GetPricedOrderAsync(string orderId)
        {
            var queryBuilder = new QueryBuilder()
                .Equalz(
                    "order-id",
                    orderId)
                .Includes("priced-order-lines");

            var priced = await _pricedOrderClient.GetListAsync(queryBuilder);

            var pricedOrderAsync = priced.ToList();
            if (pricedOrderAsync.FirstOrDefault() != null )
            {
                if (pricedOrderAsync.First().Total == 0)
                {
                    throw new OrderNotPricedException();
                }
            }

            return priced.First();
        }
    }
}