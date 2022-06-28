using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using MassTransit;

namespace Highstreetly.Payments.ReadModel
{

    /// <summary>
    /// This isn't used until we can verify b2c customer emails
    /// </summary>
    public class CustomerLinkedToStripeHandler : IConsumer<ICustomerLinkedToStripe>
    {
        private readonly Highstreetly.Infrastructure.Configuration.StripeConfiguration _stripeConfiguration;
        private readonly IAzureStorage _azureStorage;
        private PaymentsDbContext _paymentsDbContext;

        public CustomerLinkedToStripeHandler(
            Highstreetly.Infrastructure.Configuration.StripeConfiguration stripeConfiguration, 
            PaymentsDbContext paymentsDbContext,
            IAzureStorage azureStorage)
        {
            _stripeConfiguration = stripeConfiguration;
            _paymentsDbContext = paymentsDbContext;
            _azureStorage = azureStorage;
        }

        public  Task Consume(ConsumeContext<ICustomerLinkedToStripe> context)
        {
            return Task.CompletedTask;
            //  StripeConfiguration.ApiKey = _stripeConfiguration.ApiKey;
            //
            //  // attach the customer to the payment intent
            //
            //  var paymentIntents = new PaymentIntentService();
            //  var paymentMethodService = new PaymentMethodService();
            //
            //
            //  var update = new PaymentIntentUpdateOptions
            //  {
            //      Customer = context.Message.StripeCustomerId
            //  };
            //  
            // var intent =  await paymentIntents.UpdateAsync(context.Message.PaymentIntentId, update);
            //
            //  // attach the payment method to the customer:
            //  var options = new PaymentMethodAttachOptions
            //  {
            //      Customer = context.Message.StripeCustomerId
            //  };
            //  
            //  await paymentMethodService.AttachAsync(
            //      intent.PaymentMethodId,
            //      options
            //  );
        }
    }
}