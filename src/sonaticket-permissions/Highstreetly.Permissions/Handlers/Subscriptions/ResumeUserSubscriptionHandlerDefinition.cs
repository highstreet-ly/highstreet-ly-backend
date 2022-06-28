using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class ResumeUserSubscriptionHandlerDefinition : HandlerDefinitionBase<ResumeUserSubscriptionHandler>
    {
        public ResumeUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(ResumeUserSubscriptionHandler)}-handler")
        {
        }
    }
}