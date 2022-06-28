using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class OrderTotalsCalculatedHandlerDefinition : HandlerDefinitionBase<OrderTotalsCalculatedHandler>
    {
        public OrderTotalsCalculatedHandlerDefinition() : base($"payments-read-model-{nameof(OrderTotalsCalculatedHandler)}")
        {
        }
    }
}