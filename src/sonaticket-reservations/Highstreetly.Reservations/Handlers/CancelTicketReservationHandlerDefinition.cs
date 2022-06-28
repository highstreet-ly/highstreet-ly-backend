using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class CancelTicketReservationHandlerDefinition : HandlerDefinitionBase<CancelTicketReservationHandler>
    {
        public CancelTicketReservationHandlerDefinition() : base($"reservations-handlers-{nameof(CancelTicketReservationHandler)}-handler")
        {
        }
    }
}