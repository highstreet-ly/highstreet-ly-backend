using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class StartUserSubscriptionHandlerDefinition : HandlerDefinitionBase<StartUserSubscriptionHandler>
    {
        public StartUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(StartUserSubscriptionHandler)}-handler")
        {
        }
    }
}