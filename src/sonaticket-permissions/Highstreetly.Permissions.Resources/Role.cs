using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    [Resource("roles")]
    public class Role : IdentityRole<Guid>, IIdentifiable<Guid>
    {
        [NotMapped]
        public string StringId
        {
            get => Id.ToString();
            set => Id = Guid.Parse(value);
        }

       [NotMapped] public string LocalId { get; set; }

        [Attr] public bool? Privileged { get; set; }

        [Attr]
        public override string Name { get; set; }

        [Attr]
        public override string NormalizedName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        [HasManyThrough(nameof(UserRoles))]
        [NotMapped]
        public ICollection<User> Users { get; set; }
        
        [HasMany]
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }
}