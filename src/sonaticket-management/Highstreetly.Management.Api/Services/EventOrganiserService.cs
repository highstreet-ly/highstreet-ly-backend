using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Identity;
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
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Services
{
    public class EventOrganiserService : JsonApiResourceService<EventOrganiser, Guid>
    {
        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBusClient _busClient;
        private readonly ManagementDbContext _managementDbContext;
        private readonly IIdentityService _identityService;

        public EventOrganiserService(
            IResourceRepositoryAccessor repositoryAccessor,
            IQueryLayerComposer queryLayerComposer,
            IPaginationContext paginationContext,
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IJsonApiRequest request,
            IResourceChangeTracker<EventOrganiser> resourceChangeTracker,
            IResourceHookExecutorFacade hookExecutor,
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor,
            ManagementDbContext managementDbContext,
            IIdentityService identityService, 
            IBusClient busClient)
            : base(repositoryAccessor, queryLayerComposer, paginationContext, options, loggerFactory, request,
                resourceChangeTracker, hookExecutor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _managementDbContext = managementDbContext;
            _identityService = identityService;
            _busClient = busClient;
        }

        public override async Task<EventOrganiser> UpdateAsync(Guid id, EventOrganiser entity,
            CancellationToken cancellationToken)
        {
            var command = _httpContextAccessor.HttpContext.Request.Headers["Command-Type"].FirstOrDefault();

            if (command != null)
            {
                switch (command)
                {
                    case "LinkEventOrganiserAccountToStripe":
                        await _busClient.Send<ILinkEventOrganiserAccountToStripe>(new LinkEventOrganiserAccountToStripe
                        {
                            EventOrganiserId = entity.Id,
                            Code = entity.StripeCode,
                            Scope = "read_write",
                            CorrelationId = CorrelationId
                        });
                        return entity;
                }
            }

            var evtOrgModel =  _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == entity.Id);

            evtOrgModel.Description = entity.Description;
            evtOrgModel.Name = entity.Name;
            evtOrgModel.Url = entity.Url;
            evtOrgModel.LogoId = entity.LogoId;

            if (await _identityService.UserIsInRoleAsync("Admin", _httpContextAccessor.HttpContext.User))
            {
                evtOrgModel.PlatformFee = entity.PlatformFee;
                evtOrgModel.SchemaType = entity.SchemaType;
            }

            await _managementDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public override Task<EventOrganiser> CreateAsync(EventOrganiser resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected Guid CorrelationId
        {
            get
            {
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId);

                var canParse = Guid.TryParse(correlationId, out var parsed);

                if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
                {
                    return NewId.NextGuid();
                }

                return parsed;
            }
        }
    }
}