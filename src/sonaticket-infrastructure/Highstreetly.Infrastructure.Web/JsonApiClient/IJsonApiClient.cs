using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using FilterQueryParams = System.Collections.Generic.Dictionary<string, string>;

namespace Highstreetly.Infrastructure.JsonApiClient
{
    public interface IJsonApiClient<TModel, TId>
    {
        Task<IEnumerable<TModel>> GetListAsync(
            QueryBuilder queryBuilder,
            bool allowApiAuthIfNeeded = false);

        Task<TModel> GetAsync(TId id, QueryBuilder queryBuilder = default, bool allowApiAuthIfNeeded = false);

        Task<IEnumerable<TModel>> CreateAsync(IEnumerable<TModel> entities, bool allowApiAuthIfNeeded = false);

        Task<TModel> CreateAsync(TModel entity, bool allowApiAuthIfNeeded = false);

        Task<TModel> UpdateAsync(TId id, TModel entity, bool allowApiAuthIfNeeded = false);

        Task DeleteAsync(TId id, bool allowApiAuthIfNeeded = false);

        string GetModelType();

        string GetModelScope();

        string GetModelService();
    }
}