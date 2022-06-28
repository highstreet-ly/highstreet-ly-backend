using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceCreatedHandlerDefinition : HandlerDefinitionBase<EventInstanceCreatedHandler>
    {
        public EventInstanceCreatedHandlerDefinition() :base($"management-read-model-{nameof(EventInstanceCreatedHandler)}-handler")
        {
        }
    }
}