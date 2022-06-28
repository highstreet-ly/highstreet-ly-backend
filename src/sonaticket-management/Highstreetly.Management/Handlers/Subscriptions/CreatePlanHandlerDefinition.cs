using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreatePlanHandlerDefinition : HandlerDefinitionBase<CreatePlanHandler>
    {
        public CreatePlanHandlerDefinition() : base($"management-handlers-{nameof(CreatePlanHandler)}-handler")
        {
        }
    }
}