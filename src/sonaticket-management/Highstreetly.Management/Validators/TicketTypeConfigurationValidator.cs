using FluentValidation;
using Highstreetly.Management.Resources;

namespace Highstreetly.Management.Validators
{
    public class TicketTypeConfigurationValidator : AbstractValidator<TicketTypeConfiguration>
    {
        public TicketTypeConfigurationValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The name cannot be blank.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("The description cannot be blank.");
            RuleFor(x => x.Price).LessThan(100*100).WithMessage("Products must be priced < £100.");
        }
    }
}