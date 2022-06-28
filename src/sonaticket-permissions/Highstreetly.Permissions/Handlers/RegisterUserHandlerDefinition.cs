using Highstreetly.Infrastructure;

namespace Highstreetly.Permissions.Handlers
{
    public class RegisterUserHandlerDefinition : HandlerDefinitionBase<RegisterB2BUserHandler>
    {
        public RegisterUserHandlerDefinition() : base($"permissions-handlers-{nameof(RegisterB2BUserHandler)}")
        {
        }
    }
}