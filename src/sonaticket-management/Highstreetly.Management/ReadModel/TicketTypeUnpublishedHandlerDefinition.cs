using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeUnpublishedHandlerDefinition : HandlerDefinitionBase<TicketTypeUnpublishedHandler>
    {
        public TicketTypeUnpublishedHandlerDefinition() :base($"management-read-model-{nameof(TicketTypeUnpublishedHandler)}-handler")
        {
        }
    }
}