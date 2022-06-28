using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Hooks;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Serialization.Objects;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Services
{
    public class EventInstanceService : JsonApiResourceService<EventInstance, Guid>
    {
        private readonly IAuthorizationService _authService;
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ManagementDbContext _managementDbContext;

        public EventInstanceService(
            IResourceRepositoryAccessor repositoryAccessor,
            IQueryLayerComposer queryLayerComposer,
            IPaginationContext paginationContext,
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IJsonApiRequest request,
            IResourceChangeTracker<EventInstance> resourceChangeTracker,
            IResourceHookExecutorFacade hookExecutor,
            ManagementDbContext managementDbContext,
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor,
            IBusClient busClient)
            : base(
                repositoryAccessor,
                queryLayerComposer,
                paginationContext,
                options,
                loggerFactory,
                request,
                resourceChangeTracker,
                hookExecutor)
        {
            _managementDbContext = managementDbContext;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _busClient = busClient;
        }

        public override Task<EventInstance> CreateAsync(
            EventInstance resource,
            CancellationToken cancellationToken)
        {
            var evtOrgModel =
                _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == resource.EventOrganiserId);

            if (evtOrgModel == null)
            {
                var error = new Error(HttpStatusCode.NotFound)
                {
                    Detail = "There is no event organiser with that ID"
                };
                throw new JsonApiException(error);
            }

            var existingSlug =
                _managementDbContext.EventInstances
                    .Where(c => c.Slug == resource.Slug && !c.Deleted)
                    .Select(c => c.Slug)
                    .Any();

            if (existingSlug)
            {
                throw new DuplicateNameException($"The chosen ticketingEvent slug ({resource.Slug}) is already taken.");
            }

            return base.CreateAsync(
                resource,
                cancellationToken);
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            // only allow delete if there are no active orders against the IE

            var orders = await _managementDbContext.Set<Order>()
                .CountAsync(
                    x => x.EventInstanceId == id,
                    cancellationToken);

            if (orders > 0)
            {
                throw new Exception("You cannot delete an event that has active orders");
            }

            await base.DeleteAsync(
                id,
                cancellationToken);
        }
    }
}