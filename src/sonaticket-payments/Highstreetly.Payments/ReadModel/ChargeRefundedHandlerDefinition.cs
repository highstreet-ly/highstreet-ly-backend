using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeRefundedHandlerDefinition : HandlerDefinitionBase<ChargeRefundedHandler>
    {
        public ChargeRefundedHandlerDefinition() : base($"payments-read-model-{nameof(ChargeRefundedHandler)}")
        {
        }
    }
}