using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class RemoveTicketTypesHandlerDefinition : HandlerDefinitionBase<RemoveTicketTypesHandler>
    {
        public RemoveTicketTypesHandlerDefinition() : base($"reservations-handlers-{nameof(RemoveTicketTypesHandler)}-handler")
        {
        }
    }
}