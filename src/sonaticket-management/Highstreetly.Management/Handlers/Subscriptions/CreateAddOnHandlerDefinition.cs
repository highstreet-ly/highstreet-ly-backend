using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreateAddOnHandlerDefinition : HandlerDefinitionBase<CreateAddOnHandler>
    {
        public CreateAddOnHandlerDefinition() : base($"management-handlers-{nameof(CreateAddOnHandler)}-handler")
        {
        }
    }
}