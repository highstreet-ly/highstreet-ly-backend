using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class MakeTicketReservationHandlerDefinition : HandlerDefinitionBase<MakeTicketReservationHandler>
    {
        public MakeTicketReservationHandlerDefinition() : base($"reservations-handlers-{nameof(MakeTicketReservationHandler)}-handler")
        {
        }
    }
}