using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class ApplicationFeeCreatedHandlerDefinition : HandlerDefinitionBase<ApplicationFeeCreatedHandler>
    {
        public ApplicationFeeCreatedHandlerDefinition() : base($"payments-read-model-{nameof(ApplicationFeeCreatedHandler)}")
        {
        }
    }
}