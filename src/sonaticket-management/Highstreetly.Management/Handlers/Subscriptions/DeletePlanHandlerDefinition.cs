using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeletePlanHandlerDefinition : HandlerDefinitionBase<DeletePlanHandler>
    {
        public DeletePlanHandlerDefinition() : base($"management-handlers-{nameof(DeletePlanHandler)}-handler")
        {
        }
    }
}