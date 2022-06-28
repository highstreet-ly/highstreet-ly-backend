using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class InitiateThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<InitiateThirdPartyProcessorPaymentHandler>
    {
        public InitiateThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-read-model-{nameof(InitiateThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}