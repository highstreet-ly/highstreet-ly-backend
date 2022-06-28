using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstancePublishedHandlerDefinition : HandlerDefinitionBase<EventInstancePublishedHandler>
    {
        public EventInstancePublishedHandlerDefinition() :base($"management-read-model-{nameof(EventInstancePublishedHandler)}-handler")
        {
        }
    }
}