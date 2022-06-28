using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class CommitTicketReservationHandlerDefinition : HandlerDefinitionBase<CommitTicketReservationHandler>
    {
        public CommitTicketReservationHandlerDefinition() : base($"reservations-handlers-{nameof(CommitTicketReservationHandler)}-handler")
        {
        }
    }
}