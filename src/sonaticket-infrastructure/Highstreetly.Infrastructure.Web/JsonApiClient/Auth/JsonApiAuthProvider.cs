using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Infrastructure.JsonApiClient.Auth
{
    public class JsonApiAuthProvider : IJsonApiClientAuthProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JsonApiAuthProvider> _logger;

        public JsonApiAuthProvider(IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<JsonApiAuthProvider>();
        }

        public async Task<string> GetAccessTokenAsync(string scope = null, bool useApiAuth = false)
        {
            try
            {
                string accessToken = null;

                if (useApiAuth)
                {
                    _logger.LogInformation($"Requesting access on behalf of the API as specified useApiAuth is true");
                    var provider = new JsonApiNullAuthProvider(_configuration, _loggerFactory);
                    accessToken = await provider.GetAccessTokenAsync(scope, true);
                }
                else
                {
                    accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var token);
                        accessToken = token.ToString().Replace("Bearer ", string.Empty);
                    }
                }

                return accessToken;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed retrieving access token with scope {scope}");
                _logger.LogError(e.ToString());

                throw;
            }
        }
    }
}