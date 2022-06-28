using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class ChargeRefundedHandlerDefinition : HandlerDefinitionBase<ChargeRefundedHandler>
    {
        public ChargeRefundedHandlerDefinition() : base($"management-read-model-{nameof(ChargeRefundedHandler)}")
        {
        }
    }
}