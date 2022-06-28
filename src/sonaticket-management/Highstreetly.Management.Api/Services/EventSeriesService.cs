using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CommonMark;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using Highstreetly.Permissions.Contracts.Requests;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Hooks;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Claim = Highstreetly.Permissions.Contracts.Requests.Claim;
using Error = JsonApiDotNetCore.Serialization.Objects.Error;

namespace Highstreetly.Management.Api.Services
{
    public class EventSeriesService : JsonApiResourceService<EventSeries, Guid>
    {
        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBusClient _busClient;
        private readonly ManagementDbContext _managementDbContext;
        private readonly IIdentityService _identityService;
        private readonly ILogger<EventSeriesService> _logger;

        public EventSeriesService(
            IResourceRepositoryAccessor repositoryAccessor,
            IQueryLayerComposer queryLayerComposer,
            IPaginationContext paginationContext,
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IJsonApiRequest request,
            IResourceChangeTracker<EventSeries> resourceChangeTracker,
            IResourceHookExecutorFacade hookExecutor,
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor,
            IBusClient busClient,
            ManagementDbContext managementDbContext,
            IIdentityService identityService,
            ILogger<EventSeriesService> logger)
            : base(repositoryAccessor,
                queryLayerComposer,
                paginationContext,
                options,
                loggerFactory,
                request,
                resourceChangeTracker,
                hookExecutor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _busClient = busClient;
            _managementDbContext = managementDbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public override async Task<EventSeries> CreateAsync(EventSeries entity, CancellationToken cancellationToken)
        {
            if (entity.EventOrganiserId != Guid.Empty)
            {
                var evtOrgModel = _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == entity.EventOrganiserId);

                var authorizationResult = await _authService
                    .AuthorizeAsync(
                        _httpContextAccessor.HttpContext.User,
                        evtOrgModel,
                        "AdminOrOwnsOrganisationPolicy");

                if (!authorizationResult.Succeeded)
                {
                    var error = new Error(HttpStatusCode.Unauthorized)
                    {
                        Detail = "You are not authorized to carry out this request"
                    };
                    throw new JsonApiException(error);
                }
            }

            entity.Slug = entity.Name.GenerateSlug();

            var usersEoid = await EnsureUserIsEventOrganiser(entity.OwnerId.GetValueOrDefault(), entity.Name, entity.Onboarding);

            //Important: The converter does not include any HTML sanitizing.
            entity.DescriptionHtml = CommonMarkConverter.Convert(entity.Description);
            entity.Id = NewId.NextGuid();

            entity.EventOrganiserId = usersEoid;

            await base.CreateAsync(entity, cancellationToken);
            return entity;
        }

        public override async Task<EventSeries> UpdateAsync(Guid id, EventSeries entity,
            CancellationToken cancellationToken)
        {
            var es = await GetAsync(id, cancellationToken);

            if (es != null)
            {
                es.Description = entity.Description;
                es.DescriptionHtml = CommonMarkConverter.Convert(entity.Description);
                es.Name = entity.Name;

                if (entity.Featured)
                {
                    es.Featured = true;
                }

                return await base.UpdateAsync(id, entity, cancellationToken);
            }

            return entity;
        }

        public async Task<Guid> EnsureUserIsEventOrganiser(
            Guid ownerIdFromCreateForm,
            string name,
            bool entityOnboarding)
        {
            string sub;

            if (!entityOnboarding)
            {
                _logger.LogInformation("Request is not on-boarding");
                sub = _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(sub))
                {
                    sub = ownerIdFromCreateForm.ToString();
                }
            }
            else
            {
                _logger.LogInformation("Attempting onboard");
                if ( (await _identityService.UserIsInRoleAsync("Admin", _httpContextAccessor.HttpContext?.User)
                        ||// or user is a backend service
                         _httpContextAccessor.HttpContext?.User.FindFirstValue("access-all") != null))
                {
                    _logger.LogInformation("Attempting onboard: user is admin");
                    sub = ownerIdFromCreateForm.ToString();
                }
                else
                {
                    _logger.LogInformation("Attempting onboard: user is not admin");
                    var error = new Error(HttpStatusCode.Unauthorized)
                    {
                        Detail = "You are not authorized to carry out on-boarding"
                    };
                    throw new JsonApiException(error);
                }
            }

            var user = await _identityService.GetUser(sub);

            var eventOrganiserRole = await _identityService.GetRole("EventOrganiser");
            var operatorRole = await _identityService.GetRole("Operator");
            var dashUserRole = await _identityService.GetRole("DashUser");

            await _identityService.AddUserToRolesAsync(user, new List<Role>
            {
                eventOrganiserRole.Single(),
                operatorRole.Single(),
                dashUserRole.Single()
            });

            var usersEventOrganiserClaim = user.Claims.FirstOrDefault(x => x.ClaimType == "eoid");

            if (usersEventOrganiserClaim != null)
            {
                var idAsGuid = Guid.Parse(usersEventOrganiserClaim.ClaimValue);

                if (_managementDbContext.EventOrganisers.Any(x => x.Id == idAsGuid))
                {
                    _logger.LogInformation($"Event organiser for owner id {sub} already exists");
                    return Guid.Parse(usersEventOrganiserClaim.ClaimValue);
                }
            }

            var eventOrganiser = new EventOrganiser
            {
                Name = name,
                PlatformFee = Convert.ToInt32(Environment.GetEnvironmentVariable("PLATFORM_FEE_PENCE")),
                SchemaType = ApplicationSchemaType.Simple
            };

            await _managementDbContext.EventOrganisers.AddAsync(eventOrganiser);

            await _identityService.AddUserClaimAsync(user, new Claim
            {
                ClaimType = "eoid",
                ClaimValue = eventOrganiser.Id.ToString()
            });

            await _identityService.AddUserClaimAsync(user, new Claim
            {
                ClaimType = "member-of-eoid",
                ClaimValue = eventOrganiser.Id.ToString()
            });

            await _identityService.SetUserCurrentEoid(user, eventOrganiser.Id);

            var stats = new DashboardStat
            {
                Id = NewId.NextGuid(),
                EventOrganiserId = eventOrganiser.Id
            };

            await _managementDbContext.DashboardStats.AddAsync(stats);

            await _managementDbContext.SaveChangesAsync();

            return eventOrganiser.Id;
        }
    }
}