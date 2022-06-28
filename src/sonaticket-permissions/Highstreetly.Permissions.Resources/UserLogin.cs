using System;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    public class UserLogin : IdentityUserLogin<Guid>
    {
        [HasOne]
        public virtual User User { get; set; }
    }
}