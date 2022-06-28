using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Microsoft.Extensions.Caching.Memory;

namespace Highstreetly.Infrastructure.JsonApiClient
{
    public interface ICachingJsonApiClient<TModel, TId> : IJsonApiClient<TModel, TId>
    {
    }

    /// <summary>
    /// TODO: Will need to add some mechanism for invalidating the cache for each of the types that are being cached.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CachingJsonApiClient<TModel, TId> : ICachingJsonApiClient<TModel, TId>
    {
        private readonly IMemoryCache _cache;
        private readonly IJsonApiClient<TModel, TId> _inner;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly SemaphoreSlim SemaphoreSlim = new(1);
        private const int CacheForMinutes = 20;

        public CachingJsonApiClient(IJsonApiClient<TModel, TId> inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<IEnumerable<TModel>> GetListAsync(QueryBuilder queryBuilder, bool allowApiAuthIfNeeded = false)
        {
            var cacheKey = $"{_inner.GetModelService()}-{_inner.GetModelType()}-{queryBuilder.Build()}-{allowApiAuthIfNeeded}";

            if (_cache.TryGetValue<IEnumerable<TModel>>(cacheKey, out var m1))
            {
                return m1;
            }

            await SemaphoreSlim.WaitAsync();

            try
            {
                if (_cache.TryGetValue<IEnumerable<TModel>>(cacheKey, out var m2))
                {
                    return m2;
                }

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(CacheForMinutes),
                    Priority = CacheItemPriority.Normal
                };

                var results = await _inner.GetListAsync(queryBuilder, allowApiAuthIfNeeded);

                _cache.Set<IEnumerable<TModel>>(cacheKey, results, cacheExpirationOptions);

                return results;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task<TModel> GetAsync(TId id, QueryBuilder queryBuilder = default, bool allowApiAuthIfNeeded = false)
        {
            var cacheKey = $"{_inner.GetModelService()}-{_inner.GetModelType()}={id}-{queryBuilder?.Build()}-{allowApiAuthIfNeeded}";

            if (_cache.TryGetValue<TModel>(cacheKey, out var m1))
            {
                return m1;
            }

            await SemaphoreSlim.WaitAsync();

            try
            {
                if (_cache.TryGetValue<TModel>(cacheKey, out var m2))
                {
                    return m2;
                }

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(CacheForMinutes),
                    Priority = CacheItemPriority.Normal
                };

                var result = await _inner.GetAsync(id, queryBuilder, allowApiAuthIfNeeded);

                _cache.Set<TModel>(cacheKey, result, cacheExpirationOptions);

                return result;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public Task<IEnumerable<TModel>> CreateAsync(IEnumerable<TModel> entities, bool allowApiAuthIfNeeded = false)
        {
            return _inner.CreateAsync(entities, allowApiAuthIfNeeded);
        }

        public Task<TModel> CreateAsync(TModel entity, bool allowApiAuthIfNeeded = false)
        {
            return _inner.CreateAsync(entity, allowApiAuthIfNeeded);
        }

        public Task<TModel> UpdateAsync(TId id, TModel entity, bool allowApiAuthIfNeeded = false)
        {
            return _inner.UpdateAsync(id, entity, allowApiAuthIfNeeded);
        }

        public Task DeleteAsync(TId id, bool allowApiAuthIfNeeded = false)
        {
            return _inner.DeleteAsync(id, allowApiAuthIfNeeded);
        }

        public string GetModelType() => _inner.GetModelType();

        public string GetModelScope() => _inner.GetModelScope();

        public string GetModelService() => _inner.GetModelService();
    }
}