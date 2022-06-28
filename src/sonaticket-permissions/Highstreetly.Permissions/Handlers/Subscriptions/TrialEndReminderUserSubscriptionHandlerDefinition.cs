using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class TrialEndReminderUserSubscriptionHandlerDefinition : HandlerDefinitionBase<TrialEndReminderUserSubscriptionHandler>
    {
        public TrialEndReminderUserSubscriptionHandlerDefinition() : base($"permissions-handlers-{nameof(TrialEndReminderUserSubscriptionHandler)}-handler")
        {
        }
    }
}