using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderProcessingStartedHandlerDefinition : HandlerDefinitionBase<OrderProcessingStartedHandler>
    {
        public OrderProcessingStartedHandlerDefinition() :base($"management-read-model-{nameof(OrderProcessingStartedHandler)}")
        {
        }
    }
}