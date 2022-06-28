using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Permissions.Contracts.Requests
{
    [JsonObject(Title = "roles")]
    [RequestScope(Scope = Scopes.PermissionsApi)]
    [RequestService(Service = Services.PermissionsApi)]
    public class Role
    {
        public string Type { get; set; } = "roles";

        public string Id { get; set; }
        
        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }
    }
}