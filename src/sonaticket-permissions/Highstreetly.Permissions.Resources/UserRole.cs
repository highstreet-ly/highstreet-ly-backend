using System;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public override Guid UserId { get; set; }
        
        [HasOne]
        public User User { get; set; }

        public override Guid RoleId { get; set; }
        
        [HasOne]
        public Role Role { get; set; }
    }
}