using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class ResumeUserSubscriptionHandlerDefinition : HandlerDefinitionBase<ResumeUserSubscriptionHandler>
    {
        public ResumeUserSubscriptionHandlerDefinition() : base($"management-handlers-{nameof(ResumeUserSubscriptionHandler)}-handler")
        {
        }
    }
}