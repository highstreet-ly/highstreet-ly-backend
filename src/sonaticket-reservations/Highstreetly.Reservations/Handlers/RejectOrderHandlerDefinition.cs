using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class RejectOrderHandlerDefinition : HandlerDefinitionBase<RejectOrderHandler>
    {
        public RejectOrderHandlerDefinition() : base($"reservations-handlers-{nameof(RejectOrderHandler)}-handler")
        {
        }
        
    }
}