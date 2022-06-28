using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderTotalsCalculatedHandlerDefinition : HandlerDefinitionBase<OrderTotalsCalculatedHandler>
    {
        public OrderTotalsCalculatedHandlerDefinition() :base($"reservations-read-model-{nameof(OrderTotalsCalculatedHandler)}")
        {
        }
    }
}