using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Permissions.Contracts.Requests
{
    [JsonObject(Title = "claims")]
    [RequestScope(Scope = Scopes.PermissionsApi)]
    [RequestService(Service = Services.PermissionsApi)]
    public class Claim
    {
        public string Type { get; set; } = "claims";
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("claim-type")]
        public  string ClaimType { get; set; }

        [JsonProperty("claim-value")]
        public  string ClaimValue { get; set; }
        
        [JsonProperty("user")] 
        public User User { get; set; }
    }
}