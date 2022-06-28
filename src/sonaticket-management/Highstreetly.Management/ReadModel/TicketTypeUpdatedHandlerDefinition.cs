using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeUpdatedHandlerDefinition : HandlerDefinitionBase<TicketTypeUpdatedHandler>
    {
        public TicketTypeUpdatedHandlerDefinition() :base($"management-read-model-{nameof(TicketTypeUpdatedHandler)}-handler")
        {
        }
    }
}