using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentCompletedHandlerDefinition : HandlerDefinitionBase<PaymentCompletedHandler>
    {
        public PaymentCompletedHandlerDefinition() : base($"payments-read-model-{nameof(PaymentCompletedHandler)}")
        {
        }
    }
}