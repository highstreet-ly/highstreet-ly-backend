using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentInitiatedHandlerDefinition : HandlerDefinitionBase<PaymentInitiatedHandler>
    {
        public PaymentInitiatedHandlerDefinition() : base($"payments-read-model-{nameof(PaymentInitiatedHandler)}")
        {
        }
    }
}