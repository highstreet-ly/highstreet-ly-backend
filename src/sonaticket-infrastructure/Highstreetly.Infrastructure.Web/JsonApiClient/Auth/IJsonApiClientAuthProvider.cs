using System.Threading.Tasks;

namespace Highstreetly.Infrastructure.JsonApiClient.Auth
{
    public interface IJsonApiClientAuthProvider
    {
        Task<string> GetAccessTokenAsync(string scope = null, bool useApiAuth = false);
    }
}