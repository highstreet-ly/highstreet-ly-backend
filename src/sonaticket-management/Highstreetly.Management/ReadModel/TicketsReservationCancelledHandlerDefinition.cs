using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketsReservationCancelledHandlerDefinition : HandlerDefinitionBase<TicketsReservationCancelledHandler>
    {
        public TicketsReservationCancelledHandlerDefinition() :base($"management-read-model-{nameof(TicketsReservationCancelledHandler)}-handler")
        {
        }
    }
}