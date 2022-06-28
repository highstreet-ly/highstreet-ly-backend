using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators;
using Highstreetly.Management.Contracts.Requests;
using Microsoft.AspNetCore.Http;

namespace Highstreetly.Reservations.Api.Web.ResourceRepositories
{
    public static class JsonApiClientExtensions
    {
        public static async Task<List<Guid>> GetEventInstanceIdsForOrganiserId(this IJsonApiClient<EventInstance, Guid> client, IHttpContextAccessor accessor)
        {
            if (accessor
                .HttpContext == null)
            {
                throw new ArgumentException(nameof(HttpContext));
            }

            var idFilters = accessor
                .HttpContext
                .User
                .FindAll("member-of-eoid")
                .Select(x => new EqualsOperator("event-organiser-id", x.Value))
                .ToArray();

            if (idFilters.Length == 0)
            {
                return default;
            }

            QueryBuilder queryBuilder;
            if (idFilters.Length > 1)
            {
                queryBuilder = new QueryBuilder()
                    .Or(new OrOperator(idFilters.ToArray()))
                    .Fields(new SparseFieldSet(fields: "id"));
            }
            else
            {
                queryBuilder = new QueryBuilder()
                    .Equalz(idFilters.First())
                    .Fields(new SparseFieldSet(fields: "id"));
            }

            var instances
                = await client
                    .GetListAsync(queryBuilder);

            return instances.Select(x => x.Id).ToList();
        }
    }
}