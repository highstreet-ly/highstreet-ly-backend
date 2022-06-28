using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    [Resource("users")] 
    public class User : IdentityUser<Guid>, IIdentifiable<Guid>
    {
        public User()
        {
            Roles = new List<Role>();
        }
        
        [Attr]
        [Key]
        public override Guid Id { get; set; }

        [Attr]
        public override string UserName { get; set; }

        [Attr]
        public string FirstName { get; set; }

        [Attr]
        public string LastName { get; set; }

        [Attr]
        public override string Email { get; set; }

        [Attr] 
        public override string NormalizedEmail { get; set; }

        [Attr]
        public override bool EmailConfirmed { get; set; }

        [NotMapped]
        public string StringId
        {
            get => Id.ToString();
            set => Id = Guid.Parse(value);
        }

       [NotMapped] public string LocalId { get; set; }

        [Attr]
        public AvatarType? AvatarType { get; set; }

        [HasMany]
        public ICollection<Claim> Claims { get; set; }
        
        [HasMany]
        public virtual ICollection<UserLogin> Logins { get; set; }

        [HasMany]
        public virtual ICollection<UserToken> Tokens { get; set; }
        
        public ICollection<UserRole> UserRoles { get; set; }

        [NotMapped]
        [HasManyThrough(nameof(UserRoles))] 
        public ICollection<Role> Roles { get; set; }
        
        [Attr] 
        public Guid CurrentEoid { get; set; }
    }
}
