using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class CustomerLinkedToStripeHandlerDefinition : HandlerDefinitionBase<CustomerLinkedToStripeHandler>
    {
        public CustomerLinkedToStripeHandlerDefinition() : base($"payments-read-model-{nameof(CustomerLinkedToStripeHandler)}")
        {
        }
    }
}