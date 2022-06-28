using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderTotalsCalculatedHandlerDefinition : HandlerDefinitionBase<OrderTotalsCalculatedHandler>
    {
        public OrderTotalsCalculatedHandlerDefinition() :base($"management-read-model-{nameof(OrderTotalsCalculatedHandler)}")
        {
        }
    }
}