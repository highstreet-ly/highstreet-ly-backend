using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class DeliveryMethodChangedHandlerDefinition : HandlerDefinitionBase<DeliveryMethodChangedHandler>
    {
        public DeliveryMethodChangedHandlerDefinition() : base($"reservations-read-model-{nameof(DeliveryMethodChangedHandler)}-handler")
        {
        }
    }
}