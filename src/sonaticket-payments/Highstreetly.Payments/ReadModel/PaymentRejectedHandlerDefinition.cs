using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentRejectedHandlerDefinition : HandlerDefinitionBase<PaymentRejectedHandler>
    {
        public PaymentRejectedHandlerDefinition() : base($"payments-read-model-{nameof(PaymentRejectedHandler)}")
        {
        }
    }
}