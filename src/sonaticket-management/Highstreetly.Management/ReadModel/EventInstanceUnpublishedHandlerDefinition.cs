using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class EventInstanceUnpublishedHandlerDefinition : HandlerDefinitionBase<EventInstanceUnpublishedHandler>
    {
        public EventInstanceUnpublishedHandlerDefinition() :base($"management-read-model-{nameof(EventInstanceUnpublishedHandler)}-handler")
        {
        }
    }
}