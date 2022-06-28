using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class SetDeliveryMethodHandlerDefinition : HandlerDefinitionBase<SetDeliveryMethodHandler>
    {
        public SetDeliveryMethodHandlerDefinition() : base(
            $"reservations-read-model-{nameof(SetDeliveryMethodHandler)}")
        {
        }
    }
}