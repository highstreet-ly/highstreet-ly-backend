using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.Handlers
{
    public class IssueRefundHandlerDefinition : HandlerDefinitionBase<IssueRefundHandler>
    {
        public IssueRefundHandlerDefinition() : base($"payments-handlers-{nameof(IssueRefundHandler)}")
        {
        }
    }
}