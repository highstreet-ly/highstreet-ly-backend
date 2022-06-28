using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class AddTicketTypesHandlerDefinition : HandlerDefinitionBase<AddTicketTypesHandler>
    {
        public AddTicketTypesHandlerDefinition() :base($"reservations-handlers-{nameof(AddTicketTypesHandler)}-handler")
        {
        }
        
    }
}