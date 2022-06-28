using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class CreateUserSubscriptionHandlerDefinition : HandlerDefinitionBase<CreateUserSubscriptionHandler>
    {
        public CreateUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(CreateUserSubscriptionHandler)}-handler")
        {
        }
    }
}