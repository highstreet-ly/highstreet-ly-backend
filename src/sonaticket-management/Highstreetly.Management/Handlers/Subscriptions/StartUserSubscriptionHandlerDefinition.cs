using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class StartUserSubscriptionHandlerDefinition : HandlerDefinitionBase<StartUserSubscriptionHandler>
    {
        public StartUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(StartUserSubscriptionHandler)}-handler")
        {
        }
    }
}