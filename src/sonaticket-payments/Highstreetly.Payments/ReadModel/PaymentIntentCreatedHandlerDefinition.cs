using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentIntentCreatedHandlerDefinition : HandlerDefinitionBase<PaymentIntentCreatedHandler>
    {
        public PaymentIntentCreatedHandlerDefinition() : base($"payments-read-model-{nameof(PaymentIntentCreatedHandler)}")
        {
        }
    }
}