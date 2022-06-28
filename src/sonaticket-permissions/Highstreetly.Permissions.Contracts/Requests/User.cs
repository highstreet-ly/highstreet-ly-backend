using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Permissions.Contracts.Requests
{
    [JsonObject(Title = "users")]
    [RequestScope(Scope = Scopes.PermissionsApi)]
    [RequestService(Service = Services.PermissionsApi)]
    public class User 
    {
        public string Type { get; set; } = "users";
        
        public User()
        {
            Roles = new List<Role>();
        }
        
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "user-name")]
        public string UserName { get; set; }

        [JsonProperty(propertyName: "first-name")]
        public string FirstName { get; set; }

        [JsonProperty(propertyName: "last-name")]
        public string LastName { get; set; }

        [JsonProperty(propertyName: "email")]
        public string Email { get; set; }

        [JsonProperty(propertyName: "normalized-email")]
        public string NormalizedEmail { get; set; }

        [JsonProperty(propertyName: "email-confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty(propertyName: "roles")]
        public List<Role> Roles { get; set; }

        [JsonProperty(propertyName: "claims")]
        public IList<Claim> Claims { get; set; }
        
        [JsonProperty(propertyName: "current-eoid")]
        public Guid CurrentEoid { get; set; }
        
    }
}