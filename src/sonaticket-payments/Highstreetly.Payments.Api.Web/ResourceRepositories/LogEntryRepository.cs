using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Payments.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Web.ResourceRepositories
{
    public class LogEntryRepository : EntityFrameworkCoreRepository<LogEntry, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogEntryRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override IQueryable<LogEntry> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                return default;
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.GetAll() : default;
        }

    }
}