using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class ReactivateUserSubscriptionHandlerDefinition : HandlerDefinitionBase<ReactivateUserSubscriptionHandler>
    {
        public ReactivateUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(ReactivateUserSubscriptionHandler)}-handler")
        {
        }
    }
}