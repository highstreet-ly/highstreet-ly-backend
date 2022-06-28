using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class ReactivateUserSubscriptionHandlerDefinition : HandlerDefinitionBase<ReactivateUserSubscriptionHandler>
    {
        public ReactivateUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(ReactivateUserSubscriptionHandler)}-handler")
        {
        }
    }
}