using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ChargeRefundUpdatedHandlerDefinition : HandlerDefinitionBase<ChargeRefundUpdatedHandler>
    {
        public ChargeRefundUpdatedHandlerDefinition() : base($"payments-read-model-{nameof(ChargeRefundUpdatedHandler)}")
        {
        }
    }
}