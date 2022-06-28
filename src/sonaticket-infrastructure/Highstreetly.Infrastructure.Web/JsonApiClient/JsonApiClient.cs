using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using JsonApiSerializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
// ReSharper disable PossibleMultipleEnumeration

namespace Highstreetly.Infrastructure.JsonApiClient
{
    public class JsonApiClient<TModel, TId> : IJsonApiClient<TModel, TId>
    {
        private readonly ILogger<JsonApiClient<TModel, TId>> _logger;
        private readonly IJsonApiClientAuthProvider _authProvider;
        private HttpClient _httpClient;
        private readonly List<Uri> _serverUrls;
        private int _currentConfigIndex = 0;
        private readonly AsyncRetryPolicy _serverRetryPolicy;
        public readonly string ClientKey;
        public readonly string ModelType;
        private readonly IHttpClientFactory _clientFactory;

        public JsonApiClient(
            ILogger<JsonApiClient<TModel, TId>> logger,
            IJsonApiClientAuthProvider authProvider,
            IConfiguration config,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _authProvider = authProvider;
            _serverUrls = new List<Uri>();
            _clientFactory = clientFactory;
            ModelType = GetModelType();
            ClientKey = GetModelScope();
            Service = GetModelService();

            if (Environment.GetEnvironmentVariable("env") == "dev")
            {
                _serverUrls.Add(new Uri($"{config.GetSection("serviceDirectory")[Service]}/api/v1/{ModelType}"));
            }
            else
            {
                _serverUrls.Add(new Uri($"http://{Service}/api/v1/{ModelType}"));
            }

            var retries = _serverUrls.Count * 2 - 1;

            _serverRetryPolicy = Policy.Handle<Exception>()
                .RetryAsync(retries, (exception, retryCount) =>
                {
                    _logger.LogDebug($"Choosing next server - retry {retryCount}");
                    ChooseNextServer(retryCount);
                });

            _httpClient = _clientFactory.CreateClient($"{ModelType}-{Service}");
        }

        // collections
        public virtual Task<IEnumerable<TModel>> GetListAsync(QueryBuilder queryBuilder, bool allowApiAuthIfNeeded = false)
        {
            var uri = queryBuilder.Build();
            return GetListInternalAsync(uri, allowApiAuthIfNeeded);
        }

        private async Task<IEnumerable<TModel>> GetListInternalAsync(string path, bool allowApiAuthIfNeeded = false)
        {
            return await _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var serverUrl = _serverUrls[_currentConfigIndex];

                try
                {
                    await AddAuthIfAvailable(allowApiAuthIfNeeded);

                    _logger.LogInformation($"Fetching from {serverUrl}{path}");

                    var stringData = await _httpClient.GetStringAsync($"{serverUrl}{path}");

                    var result = JsonConvert.DeserializeObject<IEnumerable<TModel>>(stringData, new JsonApiSerializerSettings());
                    return result;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    throw new JsonApiClientException("{serverUrl}{path}", e);
                }
            });
        }

        // single

        public async Task<TModel> GetAsync(TId id, QueryBuilder queryBuilder = default, bool allowApiAuthIfNeeded = false)
        {
            return await GetInternalAsync(id, queryBuilder?.Build(), allowApiAuthIfNeeded);
        }

        public virtual Task<TModel> GetInternalAsync(TId id, string query, bool allowApiAuthIfNeeded = false)
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var serverUrl = _serverUrls[_currentConfigIndex];
                var uri = $"{serverUrl}/{id}{query}";

                await AddAuthIfAvailable(allowApiAuthIfNeeded);

                try
                {
                    var stringData = await _httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<TModel>(stringData, new JsonApiSerializerSettings());
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed request uri: {uri}");
                    _logger.LogError(e.ToString());
                    throw new JsonApiClientException(uri, e);
                }
            });
        }

        // POST
        public virtual async Task<IEnumerable<TModel>> CreateAsync(
            IEnumerable<TModel> entities,
            bool allowApiAuthIfNeeded = false)
        {
            return await Task.WhenAll(entities.Select(e => CreateAsync(e, allowApiAuthIfNeeded)));
        }

        public virtual Task<TModel> CreateAsync(TModel entity, bool allowApiAuthIfNeeded = false)
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var serverUrl = _serverUrls[_currentConfigIndex];
                await AddAuthIfAvailable(allowApiAuthIfNeeded);
                var postData = JsonConvert.SerializeObject(entity, new JsonApiSerializerSettings());

                var httpMethod = new HttpMethod("POST");
                var request = new HttpRequestMessage(httpMethod, serverUrl) { Content = new StringContent(postData) };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

                HttpResponseMessage postResult;
                try
                {
                    postResult = await _httpClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new ApiClientException(serverUrl.ToString(), "POST");
                }

                if (postResult.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"PostAsync completed for ${ModelType}");
                }
                else
                {
                    var reason =
                        $"POSTing to {ModelType} failed with status code {postResult.StatusCode} and reason {postResult.ReasonPhrase}";
                    _logger.LogError(reason);
                    _logger.LogError(await postResult.Content.ReadAsStringAsync());
                    throw new ApiClientException(serverUrl.ToString(), "POST", reason);
                }

                var deserializeObject =
                    JsonConvert.DeserializeObject<TModel>(await postResult.Content.ReadAsStringAsync(),
                        new JsonApiSerializerSettings());
                return deserializeObject;
            });
        }

        public virtual Task<TModel> UpdateAsync(TId id, TModel entity, bool allowApiAuthIfNeeded = false)
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {

                var serverUrl = _serverUrls[_currentConfigIndex];
                await AddAuthIfAvailable(allowApiAuthIfNeeded);
                var patchData = JsonConvert.SerializeObject(entity, new JsonApiSerializerSettings());

                var httpMethod = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(httpMethod, $"{serverUrl}/{id}")
                {
                    Content = new StringContent(patchData)
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

                HttpResponseMessage postResult;
                try
                {
                    postResult = await _httpClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new JsonApiClientException(serverUrl.ToString(), e);
                }

                if (postResult.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"PatchAsync completed for ${ModelType}");
                }
                else
                {
                    var reason =
                        $"PatchAsync to ${ModelType} failed with status code ${postResult.StatusCode} and reason ${postResult.ReasonPhrase}";
                    _logger.LogError(reason);
                    throw new Exception(reason);
                }

                var deserializeObject =
                    JsonConvert.DeserializeObject<TModel>(await postResult.Content.ReadAsStringAsync(),
                        new JsonApiSerializerSettings());
                return deserializeObject;
            });
        }

        public Task DeleteAsync(TId id, bool allowApiAuthIfNeeded = false)
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                _httpClient = _clientFactory.CreateClient($"{ModelType}-{Service}");
                var serverUrl = _serverUrls[_currentConfigIndex];
                var uri = $"{serverUrl}/{id}";
                await AddAuthIfAvailable(allowApiAuthIfNeeded);

                var httpMethod = new HttpMethod("DELETE");
                var request = new HttpRequestMessage(httpMethod, uri);

                HttpResponseMessage deleteResult;
                try
                {
                    deleteResult = await _httpClient.SendAsync(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new JsonApiClientException(uri, e);
                }

                if (deleteResult.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"PostAsync completed for ${ModelType}");
                }
                else
                {
                    var reason =
                        $"DELETE-ing to {ModelType} failed with status code {deleteResult.StatusCode} and reason {deleteResult.ReasonPhrase}";
                    _logger.LogError(reason);
                    _logger.LogError(await deleteResult.Content.ReadAsStringAsync());
                    throw new Exception(reason);
                }

            });
        }

        public string Service { get; set; }

        private void ChooseNextServer(int retryCount)
        {
            if (retryCount % 2 == 0)
            {
                _logger.LogWarning("Trying next server... \n");
                _currentConfigIndex++;

                if (_currentConfigIndex > _serverUrls.Count - 1)
                    _currentConfigIndex = 0;
            }
        }

        public string GetModelType()
        {
            var attrs = System.Attribute.GetCustomAttributes(typeof(TModel));
            var jsonObjectAttribute = (JsonObjectAttribute)attrs.Single(x => x is JsonObjectAttribute);
            return jsonObjectAttribute.Title;
        }

        public string GetModelScope()
        {
            var attrs = System.Attribute.GetCustomAttributes(typeof(TModel));
            var jsonObjectAttribute = (RequestScopeAttribute)attrs.Single(x => x is RequestScopeAttribute);
            return jsonObjectAttribute.Scope;
        }

        public string GetModelService()
        {
            var attrs = System.Attribute.GetCustomAttributes(typeof(TModel));
            var jsonObjectAttribute = (RequestServiceAttribute)attrs.Single(x => x is RequestServiceAttribute);
            return jsonObjectAttribute.Service;
        }

        protected async Task<bool> AddAuthIfAvailable(bool allowApiAuthIfNeeded = false)
        {
            var authIsAvailable = false;
            if (_authProvider != null)
            {
                _logger.LogDebug(
                    $"We have an auth provider so fetching the token for ClientKey: {ClientKey}");
                var token = await _authProvider.GetAccessTokenAsync(ClientKey, allowApiAuthIfNeeded);
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    authIsAvailable = true;
                }
            }

            return authIsAvailable;
        }
    }
}