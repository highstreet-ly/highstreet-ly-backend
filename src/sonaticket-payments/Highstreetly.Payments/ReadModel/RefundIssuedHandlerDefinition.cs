using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class RefundIssuedHandlerDefinition : HandlerDefinitionBase<RefundIssuedHandler>
    {
        public RefundIssuedHandlerDefinition() : base($"payments-read-model-{nameof(RefundIssuedHandler)}")
        {
        }
    }
}