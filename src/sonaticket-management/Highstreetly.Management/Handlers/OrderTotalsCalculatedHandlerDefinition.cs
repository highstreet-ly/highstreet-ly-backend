using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class OrderTotalsCalculatedHandlerDefinition : HandlerDefinitionBase<OrderTotalsCalculatedHandler>
    {
        public OrderTotalsCalculatedHandlerDefinition() :base($"management-handlers-{nameof(OrderTotalsCalculatedHandler)}-handler")
        {
        }
    }
}