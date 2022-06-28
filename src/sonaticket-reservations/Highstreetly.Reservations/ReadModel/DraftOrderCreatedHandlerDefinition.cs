using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class DraftOrderCreatedHandlerDefinition : HandlerDefinitionBase<DraftOrderCreatedHandler>
    {
        public DraftOrderCreatedHandlerDefinition() : base($"reservations-read-model-{nameof(DraftOrderCreatedHandler)}-handler")
        {
        }
    }
}