using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class CancelThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<CancelThirdPartyProcessorPaymentHandler>
    {
        public CancelThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-read-model-{nameof(CancelThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}