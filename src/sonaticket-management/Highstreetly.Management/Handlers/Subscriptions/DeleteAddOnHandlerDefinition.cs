using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeleteAddOnHandlerDefinition : HandlerDefinitionBase<DeleteAddOnHandler>
    {
        public DeleteAddOnHandlerDefinition() : base($"management-handlers-{nameof(DeleteAddOnHandler)}-handler")
        {
        }
    }
}