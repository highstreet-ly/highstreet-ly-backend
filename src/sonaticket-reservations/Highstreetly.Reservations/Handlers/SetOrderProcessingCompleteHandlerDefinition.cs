using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class SetOrderProcessingCompleteHandlerDefinition : HandlerDefinitionBase<SetOrderProcessingCompleteHandler>
    {
        public SetOrderProcessingCompleteHandlerDefinition() : base(
            $"reservations-read-model-{nameof(SetOrderProcessingCompleteHandler)}")
        {
        }
    }
}