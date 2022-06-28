using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class UnPublishEventInstanceHandlerDefinition : HandlerDefinitionBase<UnPublishEventInstanceHandler>
    {
        public UnPublishEventInstanceHandlerDefinition() :base($"management-handlers-{nameof(UnPublishEventInstanceHandler)}-handler")
        {
        }
    }
}