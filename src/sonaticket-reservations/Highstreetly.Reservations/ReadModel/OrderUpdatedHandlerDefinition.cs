using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderUpdatedHandlerDefinition : HandlerDefinitionBase<OrderUpdatedHandler>
    {
        public OrderUpdatedHandlerDefinition() :base($"reservations-read-model-{nameof(OrderUpdatedHandler)}-handler")
        {
        }
        
    }
}