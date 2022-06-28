using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderExpiredHandlerDefinition : HandlerDefinitionBase<OrderExpiredHandler>
    {
        public OrderExpiredHandlerDefinition() : base($"reservations-read-model-{nameof(OrderExpiredHandler)}-handler")
        {
        }
        
    }
}