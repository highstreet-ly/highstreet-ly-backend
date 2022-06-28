using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentSucceededHandlerDefinition : HandlerDefinitionBase<PaymentIntentSucceededHandler>
    {
        public PaymentIntentSucceededHandlerDefinition() : base($"payments-read-model-{nameof(PaymentIntentSucceededHandler)}")
        {
        }
    }
}