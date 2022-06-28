using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Highstreetly.Permissions.Resources
{
    [Resource("claims")]
    public class Claim : IdentityUserClaim<Guid>, IIdentifiable<int>
    {

        [NotMapped]
        public string StringId
        {
            get => Id.ToString();
            set => Id = Convert.ToInt32(value);
        }

        [NotMapped] public string LocalId { get; set; }

        [Attr]
        public override string ClaimType { get; set; }

        [Attr]
        public override string ClaimValue { get; set; }

        [HasOne]
        public User User { get; set; }

    }
}