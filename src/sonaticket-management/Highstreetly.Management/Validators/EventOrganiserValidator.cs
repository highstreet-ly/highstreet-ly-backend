using System.Linq;
using FluentValidation;
using Highstreetly.Management.Resources;

namespace Highstreetly.Management.Validators
{
    public class EventOrganiserValidator : AbstractValidator<EventOrganiser>
    {
        public EventOrganiserValidator(ManagementDbContext managementDbContext)
        {
            RuleFor(x => x.Url).Custom((s, context) =>
            {
                var entity = ((EventOrganiser) context.InstanceToValidate);

                if (!string.IsNullOrEmpty(entity.Url) && managementDbContext.EventOrganisers
                    .Count(x => x.Url == entity.Url && x.Id != entity.Id) > 0)
                {
                    context.AddFailure("Duplicate Url - the event organiser vanity URL is already taken");
                }
            });

            RuleFor(x => x.Name).Custom((s, context) =>
            {
                var entity = ((EventOrganiser)context.InstanceToValidate);

                if (!string.IsNullOrEmpty(entity.Name) && managementDbContext.EventOrganisers
                    .Count(x => x.Name == entity.Name && x.Id != entity.Id) > 0)
                {
                    context.AddFailure("Duplicate Name - the event organiser name is already taken");
                }
            });
        }
    }
}