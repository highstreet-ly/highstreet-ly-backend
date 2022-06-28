using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeCreatedHandlerDefinition : HandlerDefinitionBase<TicketTypeCreatedHandler>
    {
        public TicketTypeCreatedHandlerDefinition() : base($"management-read-model-{nameof(TicketTypeCreatedHandler)}-handler")
        {
        }
    }
}