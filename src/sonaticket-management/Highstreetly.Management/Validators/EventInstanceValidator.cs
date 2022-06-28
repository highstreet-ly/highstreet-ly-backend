using System.Linq;
using FluentValidation;
using Highstreetly.Management.Resources;
using PhoneNumbers;

namespace Highstreetly.Management.Validators
{
    public class EventInstanceValidator : AbstractValidator<EventInstance>
    {
        public EventInstanceValidator(ManagementDbContext documentStoreClient)
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();

            RuleFor(x => x.Name).NotEmpty().WithMessage("The name cannot be blank.");
            // RuleFor(x => x.Location).NotEmpty().WithMessage("The location cannot be blank.");
            // RuleFor(x => x.PostCode).NotEmpty().WithMessage("The postcode cannot be blank.");
            // RuleFor(x => x.DeliveryRadiusMiles).NotEmpty().When(x => x.IsLocalDelivery).WithMessage("The delivery radius in miles cannot be blank.");
            RuleFor(x => x.NotificationEmail).EmailAddress();
            RuleFor(x => x.SupportEmail).EmailAddress();

            RuleFor(x => x.NotificationPhone)
                .Custom((s, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        var number = phoneNumberUtil.Parse(s, "GB");
                        if (!phoneNumberUtil.IsValidNumber(number))
                        {
                            context.AddFailure("Invalid value for notification phone number");
                        }
                    }
                });

            RuleFor(x => x.SupportPhone)
                .Custom((s, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        var number = phoneNumberUtil.Parse(s, "GB");
                        if (!phoneNumberUtil.IsValidNumber(number))
                        {
                            context.AddFailure("Invalid value for business phone number");
                        }
                    }
                });

            RuleFor(x => x.Name).Custom((s, context) =>
            {
                var entity = ((EventInstance)context.InstanceToValidate);

                var existing = documentStoreClient.EventInstances
                    .Where(x => x.Name == entity.Name && x.Id != entity.Id && !x.Deleted);

                if (!string.IsNullOrEmpty(entity.Name)
                    && existing.Any())
                {
                    context.AddFailure("Duplicate Name - the event name is already taken");
                }
            });
        }
    }
}
