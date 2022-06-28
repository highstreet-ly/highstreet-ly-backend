using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceRepositories
{
    public class LogEntryRepository : EntityFrameworkCoreRepository<LogEntry, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogEntryRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task UpdateAsync(LogEntry resourceFromRequest, LogEntry resourceFromDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken) : throw new UnauthorizedAccessException();
        }

        public override Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.DeleteAsync(id, cancellationToken) : throw new UnauthorizedAccessException();
        }

        protected override IQueryable<LogEntry> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.GetAll() : throw new UnauthorizedAccessException();
        }
    }
}