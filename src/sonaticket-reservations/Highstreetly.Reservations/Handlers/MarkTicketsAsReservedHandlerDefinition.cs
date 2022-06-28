using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class MarkTicketsAsReservedHandlerDefinition : HandlerDefinitionBase<MarkTicketsAsReservedHandler>
    {
        public MarkTicketsAsReservedHandlerDefinition() : base($"reservations-handlers-{nameof(MarkTicketsAsReservedHandler)}-handler")
        {
        }
    }
}