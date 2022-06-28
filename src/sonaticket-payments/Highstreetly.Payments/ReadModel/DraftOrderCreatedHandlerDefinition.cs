using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class DraftOrderCreatedHandlerDefinition : HandlerDefinitionBase<DraftOrderCreatedHandler>
    {
        public DraftOrderCreatedHandlerDefinition() : base($"payments-read-model-{nameof(DraftOrderCreatedHandler)}")
        {
        }
    }
}