using System;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Payments.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentCreatedHandler : IConsumer<IPaymentIntentCreated>
    {
        private readonly ILogger<PaymentIntentCreatedHandler> _logger;
        private readonly IAzureStorage _azureStorage;
        private readonly PaymentsDbContext _paymentsDbContext;
        
        public PaymentIntentCreatedHandler(ILogger<PaymentIntentCreatedHandler> logger, IAzureStorage azureStorage, PaymentsDbContext paymentsDbContext)
        {
            _logger = logger;
            _azureStorage = azureStorage;
            _paymentsDbContext = paymentsDbContext;
        }

        public async Task Consume(ConsumeContext<IPaymentIntentCreated> context)
        {
            var stripeEvent = await _azureStorage.ReadJsonPayloadFromAuzureBlob(context.Message.HsEventId);

             var paymentIntent = JsonConvert.DeserializeObject<PaymentIntent>(stripeEvent);

             var payment = _paymentsDbContext
                           .Payments
                           .Include(x => x.Charges)
                           .First(x => x.PaymentIntentId == paymentIntent.Id);
             
             foreach (var paymentIntentCharge in paymentIntent.Charges.Data)
             {
                 var charge = payment.Charges.FirstOrDefault(x => x.ChargeId == paymentIntentCharge.Id);
                 if (charge == null)
                 {
                     payment.Charges.Add(new Resources.Charge()
                                         {
                                             Amount = paymentIntentCharge.Amount,
                                             Application = paymentIntentCharge.Application.Id,
                                             Currency = paymentIntentCharge.Currency,
                                             Description = paymentIntentCharge.Description,
                                             DateCreated = DateTime.UtcNow
                                         });
                 }
             }

             await _paymentsDbContext.SaveChangesAsync();
        }
    }
}