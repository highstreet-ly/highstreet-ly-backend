using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ApplicationFeeRefundedHandlerDefinition : HandlerDefinitionBase<ApplicationFeeRefundedHandler>
    {
        public ApplicationFeeRefundedHandlerDefinition() : base($"payments-read-model-{nameof(ApplicationFeeRefundedHandler)}")
        {
        }
    }
}