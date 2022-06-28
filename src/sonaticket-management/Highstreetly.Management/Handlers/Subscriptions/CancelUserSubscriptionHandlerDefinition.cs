using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CancelUserSubscriptionHandlerDefinition : HandlerDefinitionBase<CancelUserSubscriptionHandler>
    {
        public CancelUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(CancelUserSubscriptionHandler)}-handler")
        {
        }
    }
}