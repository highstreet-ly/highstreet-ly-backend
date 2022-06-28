using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceUpdatedHandlerDefinition : HandlerDefinitionBase<EventInstanceUpdatedHandler>
    {
        public EventInstanceUpdatedHandlerDefinition() :base($"management-read-model-{nameof(EventInstanceUpdatedHandler)}-handler")
        {
        }
    }
}