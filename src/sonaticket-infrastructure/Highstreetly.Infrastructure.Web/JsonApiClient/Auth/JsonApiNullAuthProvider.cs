using System;
using System.Net.Http;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Caching.Memory;
using Polly.Retry;
using Polly.Wrap;

namespace Highstreetly.Infrastructure.JsonApiClient.Auth
{
    public class JsonApiNullAuthProvider : IJsonApiClientAuthProvider
    {
        private readonly IConfiguration _config;
        private readonly ILogger<JsonApiNullAuthProvider> _logger;
        private readonly AsyncPolicyWrap _cachedExecutionPolicy;
        private AsyncRetryPolicy _serverRetryPolicy;

        public JsonApiNullAuthProvider(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<JsonApiNullAuthProvider>();

            var retries = 1 * 2 - 1;
            var pauseBetweenFailures = TimeSpan.FromSeconds(1);

            _serverRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retries, i => pauseBetweenFailures);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            var cache = Policy.CacheAsync(memoryCacheProvider, TimeSpan.FromMinutes(60));

            _cachedExecutionPolicy = Policy.WrapAsync(cache, _serverRetryPolicy);
        }

        public async Task<string> GetAccessTokenAsync(string scope = null, bool useApiAuth = false)
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var skip = _config.GetSection("IdentityServer")["SkipClientAuthConfiguration"];

            if (!string.IsNullOrEmpty(skip) && skip.ToLower() == "true")
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    var disco =  await _cachedExecutionPolicy.ExecuteAsync(async (c) =>
                    {
                        var req = new DiscoveryDocumentRequest
                        {
                            Address = _config.GetIdsUrl()
                        };

                        var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync(req);
                        if (!discoveryDocumentResponse.IsError) return discoveryDocumentResponse.TokenEndpoint;

                        _logger.LogError($"Error fetching discover document {discoveryDocumentResponse.Error}");

                        throw new Exception(discoveryDocumentResponse.Error);

                    }, new Context($"disco-document"));

                    var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
                    {
                        Address = disco,
                        ClientId = _config.GetSection("IdentityServer")["ClientId"],
                        ClientSecret = _config.GetSection("IdentityServer")["ClientSecret"],
                        Scope = scope,
                        ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
                    };

                    var tokenResponse =
                       await _serverRetryPolicy.ExecuteAsync(async () =>
                            await client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest));

                    if (tokenResponse.IsError)
                    {
                        _logger.LogError($"Error fetching token {tokenResponse.Error}");
                        throw new Exception(tokenResponse.Error);
                    }

                    return tokenResponse.AccessToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}