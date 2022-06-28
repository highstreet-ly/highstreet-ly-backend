using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.Handlers
{
    public class CompleteThirdPartyProcessorPaymentHandlerDefinition : HandlerDefinitionBase<CompleteThirdPartyProcessorPaymentHandler>
    {
        public CompleteThirdPartyProcessorPaymentHandlerDefinition() : base($"payments-handlers-{nameof(CompleteThirdPartyProcessorPaymentHandler)}-handler")
        {
        }
    }
}