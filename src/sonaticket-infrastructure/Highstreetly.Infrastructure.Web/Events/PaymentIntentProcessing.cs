using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class PaymentIntentProcessing :StripeEvent,  IPaymentIntentProcessing
    {
        
    }
}