using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentProcessingHandlerDefinition : HandlerDefinitionBase<PaymentIntentProcessingHandler>
    {
        public PaymentIntentProcessingHandlerDefinition() : base($"payments-read-model-{nameof(PaymentIntentProcessingHandler)}")
        {
        }
    }
}