using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class UpdateAddOnHandlerDefinition : HandlerDefinitionBase<UpdateAddOnHandler>
    {
        public UpdateAddOnHandlerDefinition() : base($"management-handlers-{nameof(UpdateAddOnHandler)}-handler")
        {
        }
    }
}