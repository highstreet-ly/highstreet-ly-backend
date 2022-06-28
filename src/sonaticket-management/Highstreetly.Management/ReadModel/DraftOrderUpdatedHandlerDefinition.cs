using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class DraftOrderUpdatedHandlerDefinition : HandlerDefinitionBase<DraftOrderUpdatedHandler>
    {
        public DraftOrderUpdatedHandlerDefinition() :base($"management-read-model-{nameof(DraftOrderUpdatedHandler)}")
        {
        }
    }
}