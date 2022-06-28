using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class PauseUserSubscriptionHandlerDefinition : HandlerDefinitionBase<PauseUserSubscriptionHandler>
    {
        public PauseUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(PauseUserSubscriptionHandler)}-handler")
        {
        }
    }
}