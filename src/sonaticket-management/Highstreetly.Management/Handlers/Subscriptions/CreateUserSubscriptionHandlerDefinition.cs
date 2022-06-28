using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreateUserSubscriptionHandlerDefinition : HandlerDefinitionBase<CreateUserSubscriptionHandler>
    {
        public CreateUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(CreateUserSubscriptionHandler)}-handler")
        {
        }
    }
}