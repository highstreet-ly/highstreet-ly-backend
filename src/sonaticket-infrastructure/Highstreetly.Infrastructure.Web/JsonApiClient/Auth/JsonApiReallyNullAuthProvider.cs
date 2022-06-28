using System.Threading.Tasks;

namespace Highstreetly.Infrastructure.JsonApiClient.Auth
{
    public class JsonApiReallyNullAuthProvider : IJsonApiClientAuthProvider
    {
        public Task<string> GetAccessTokenAsync(string scope = null, bool useApiAuth = false)
        {
            return Task.FromResult("");
        }
    }
}