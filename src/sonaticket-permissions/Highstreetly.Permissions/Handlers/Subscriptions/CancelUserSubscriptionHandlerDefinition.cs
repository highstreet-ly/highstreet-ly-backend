using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class CancelUserSubscriptionHandlerDefinition : HandlerDefinitionBase<CancelUserSubscriptionHandler>
    {
        public CancelUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(CancelUserSubscriptionHandler)}-handler")
        {
        }
    }
}