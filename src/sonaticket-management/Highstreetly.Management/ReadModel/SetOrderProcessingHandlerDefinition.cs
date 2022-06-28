using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class SetOrderProcessingHandlerDefinition : HandlerDefinitionBase<SetOrderProcessingHandler>
    {
        public SetOrderProcessingHandlerDefinition() : base(
            $"management-read-model-{nameof(SetOrderProcessingHandler)}")
        {
        }
    }
}