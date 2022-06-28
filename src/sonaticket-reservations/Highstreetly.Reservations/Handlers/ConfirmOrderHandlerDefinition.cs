using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class ConfirmOrderHandlerDefinition : HandlerDefinitionBase<ConfirmOrderHandler>
    {
        public ConfirmOrderHandlerDefinition() : base($"reservations-handlers-{nameof(ConfirmOrderHandler)}-handler")
        {
        }
    }
}