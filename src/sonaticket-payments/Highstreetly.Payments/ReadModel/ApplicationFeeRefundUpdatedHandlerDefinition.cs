using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ApplicationFeeRefundUpdatedHandlerDefinition : HandlerDefinitionBase<ApplicationFeeRefundUpdatedHandler>
    {
        public ApplicationFeeRefundUpdatedHandlerDefinition() : base($"payments-read-model-{nameof(ApplicationFeeRefundUpdatedHandler)}")
        {
        }
    }
}