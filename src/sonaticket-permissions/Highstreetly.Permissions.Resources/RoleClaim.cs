using System;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public override Guid RoleId { get; set; }
        
        [HasOne]
        public Role Role { get; set; }
    }
}