using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class UnsetEventAsFeaturedHandlerDefinition : HandlerDefinitionBase<UnsetEventAsFeaturedHandler>
    {
        public UnsetEventAsFeaturedHandlerDefinition() :base($"management-read-model-{nameof(UnsetEventAsFeaturedHandler)}-handler")
        {
        }
    }
}