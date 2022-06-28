using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class UpdatePlanHandlerDefinition : HandlerDefinitionBase<UpdatePlanHandler>
    {
        public UpdatePlanHandlerDefinition() : base($"management-handlers-{nameof(UpdatePlanHandler)}-handler")
        {
        }
    }
}