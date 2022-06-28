using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypePublishedHandlerDefinition : HandlerDefinitionBase<TicketTypePublishedHandler>
    {
        public TicketTypePublishedHandlerDefinition() :base($"management-read-model-{nameof(TicketTypePublishedHandler)}-handler")
        {
        }
    }
}