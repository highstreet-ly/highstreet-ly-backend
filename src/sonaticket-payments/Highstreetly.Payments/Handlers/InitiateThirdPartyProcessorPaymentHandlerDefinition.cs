using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.Handlers
{
    public class InitiateThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<InitiateThirdPartyProcessorPaymentHandler>
    {
        public InitiateThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-handlers-{nameof(InitiateThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}