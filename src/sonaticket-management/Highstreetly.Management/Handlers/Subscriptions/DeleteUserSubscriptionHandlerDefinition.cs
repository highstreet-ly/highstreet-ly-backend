using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeleteUserSubscriptionHandlerDefinition : HandlerDefinitionBase<DeleteUserSubscriptionHandler>
    {
        public DeleteUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(DeleteUserSubscriptionHandler)}-handler")
        {
        }
    }
}