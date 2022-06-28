using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class PaymentInitiatedHandlerDefinition : HandlerDefinitionBase<PaymentInitiatedHandler>
    {
        public PaymentInitiatedHandlerDefinition() : base($"management-read-model-{nameof(PaymentInitiatedHandler)}")
        {
        }
    }
}