using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class PublishEventInstanceHandlerDefinition : HandlerDefinitionBase<PublishEventInstanceHandler>
    {
        public PublishEventInstanceHandlerDefinition() :base($"management-handlers-{nameof(PublishEventInstanceHandler)}-handler")
        {
        }
    }
}