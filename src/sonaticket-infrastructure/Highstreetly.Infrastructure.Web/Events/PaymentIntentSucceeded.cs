using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class PaymentIntentSucceeded : StripeEvent, IPaymentIntentSucceeded
    {
       
    }
}