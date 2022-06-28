using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class DeleteUserSubscriptionHandlerDefinition : HandlerDefinitionBase<DeleteUserSubscriptionHandler>
    {
        public DeleteUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(DeleteUserSubscriptionHandler)}-handler")
        {
        }
    }
}