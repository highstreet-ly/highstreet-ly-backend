using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeSucceededHandlerDefinition : HandlerDefinitionBase<ChargeSucceededHandler>
    {
        public ChargeSucceededHandlerDefinition() : base($"payments-read-model-{nameof(ChargeSucceededHandler)}")
        {
        }
    }
}