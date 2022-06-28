using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class SetOrderProcessingHandlerDefinition : HandlerDefinitionBase<SetOrderProcessingHandler>
    {
        public SetOrderProcessingHandlerDefinition() : base(
            $"reservations-read-model-{nameof(SetOrderProcessingHandler)}")
        {
        }
    }
}