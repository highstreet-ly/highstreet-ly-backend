using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.Handlers
{
    public class CancelThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<CancelThirdPartyProcessorPaymentHandler>
    {
        public CancelThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-handlers-{nameof(CancelThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}