using System;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    [Resource("user-tokens")] 
    public class UserToken : IdentityUserToken<Guid>
    {
        [HasOne]
        public  User User { get; set; }
    }
}