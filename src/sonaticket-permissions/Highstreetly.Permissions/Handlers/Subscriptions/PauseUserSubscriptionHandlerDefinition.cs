using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class PauseUserSubscriptionHandlerDefinition : HandlerDefinitionBase<PauseUserSubscriptionHandler>
    {
        public PauseUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(PauseUserSubscriptionHandler)}-handler")
        {
        }
    }
}