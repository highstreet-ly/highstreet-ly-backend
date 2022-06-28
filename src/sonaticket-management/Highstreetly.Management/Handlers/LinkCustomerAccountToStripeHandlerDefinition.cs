using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class LinkCustomerAccountToStripeHandlerDefinition : HandlerDefinitionBase<LinkCustomerAccountToStripeHandler>
    {
        public LinkCustomerAccountToStripeHandlerDefinition() : base($"management-handlers-{nameof(LinkCustomerAccountToStripeHandler)}-handler")
        {
        }

    }
}