using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class LinkEventOrganiserAccountToStripeHandlerDefinition : HandlerDefinitionBase<LinkEventOrganiserAccountToStripeHandler>
    {
        public LinkEventOrganiserAccountToStripeHandlerDefinition() :base($"management-handlers-{nameof(LinkEventOrganiserAccountToStripeHandler)}-handler")
        {
        }
        
    }
}