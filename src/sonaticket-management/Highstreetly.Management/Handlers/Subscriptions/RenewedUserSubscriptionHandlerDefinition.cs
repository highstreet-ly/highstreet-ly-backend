using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class RenewedUserSubscriptionHandlerDefinition : HandlerDefinitionBase<RenewedUserSubscriptionHandler>
    {
        public RenewedUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(RenewedUserSubscriptionHandler)}-handler")
        {
        }
    }
}