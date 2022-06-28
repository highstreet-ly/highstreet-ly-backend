using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderProcessingCompletedHandlerDefinition : HandlerDefinitionBase<OrderProcessingCompletedHandler>
    {
        public OrderProcessingCompletedHandlerDefinition() :base($"management-read-model-{nameof(OrderProcessingCompletedHandler)}")
        {
        }
    }
}