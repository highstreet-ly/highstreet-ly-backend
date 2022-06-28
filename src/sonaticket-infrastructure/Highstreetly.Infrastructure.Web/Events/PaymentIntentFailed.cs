using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class PaymentIntentFailed :StripeEvent,  IPaymentIntentFailed
    {
        
    }
}