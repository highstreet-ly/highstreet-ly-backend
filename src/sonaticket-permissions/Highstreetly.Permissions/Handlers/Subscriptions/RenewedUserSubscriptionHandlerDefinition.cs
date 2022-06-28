using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class RenewedUserSubscriptionHandlerDefinition : HandlerDefinitionBase<RenewedUserSubscriptionHandler>
    {
        public RenewedUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(RenewedUserSubscriptionHandler)}-handler")
        {
        }
    }
}