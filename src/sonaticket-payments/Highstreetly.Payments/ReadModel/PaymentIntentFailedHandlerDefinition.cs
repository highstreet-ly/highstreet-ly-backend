using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentFailedHandlerDefinition : HandlerDefinitionBase<PaymentIntentFailedHandler>
    {
        public PaymentIntentFailedHandlerDefinition() : base($"payments-read-model-{nameof(PaymentIntentFailedHandler)}")
        {
        }
    }
}