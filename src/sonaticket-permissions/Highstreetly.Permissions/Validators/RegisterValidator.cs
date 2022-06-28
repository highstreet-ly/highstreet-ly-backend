using FluentValidation;
using Highstreetly.Permissions.Resources;

namespace Highstreetly.Permissions.Validators
{
    public class RegisterValidator : AbstractValidator<Register>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("The email address must be valid and cannot be blank.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("The password cannot be blank.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("The password cannot be blank.");
        }
    }
}
