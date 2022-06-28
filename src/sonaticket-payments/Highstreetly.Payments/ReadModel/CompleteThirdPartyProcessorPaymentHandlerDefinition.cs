using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class CompleteThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<CompleteThirdPartyProcessorPaymentHandler>
    {
        public CompleteThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-read-model-{nameof(CompleteThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}