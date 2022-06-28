using System;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Permissions.Resources
{
    [Resource("register")]
    public class Register : Identifiable<string>
    {
        [Attr]
        public string Email { get; set; }

        [Attr]
        public string Password { get; set; }

        [Attr]
        public string ConfirmPassword { get; set; }

        [Attr]
        public string UserName { get; set; }

        [Attr]
        public string FirstName { get; set; }

        [Attr]
        public string LastName { get; set; }

        [Attr]
        public string Redirect { get; internal set; }

        [Attr]
        public string createEventName { get; set; }

        [Attr]
        public DateTime createEventStartDate { get; set; }

        [Attr]
        public bool Onboarding { get; set; }

        [Attr] public Guid? CreateEventBusinessTypeId { get; set; }
    }
}