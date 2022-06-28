using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderConfirmedHandlerDefinition : HandlerDefinitionBase<OrderConfirmedHandler>
    {
        public OrderConfirmedHandlerDefinition() :base($"management-read-model-{nameof(OrderConfirmedHandler)}-handler")
        {
        }
    }
}