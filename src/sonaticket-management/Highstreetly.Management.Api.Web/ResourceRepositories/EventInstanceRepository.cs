using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventInstance = Highstreetly.Management.Resources.EventInstance;

namespace Highstreetly.Management.Api.Web.ResourceRepositories
{
    public class EventInstanceRepository : EntityFrameworkCoreRepository<EventInstance, Guid>
    {
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _managementDbContext;
        private readonly IResourceDefinitionAccessor _resourceDefinitionAccessor;

        public EventInstanceRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor,
            IBusClient busClient) : base(
            targetedFields,
            contextResolver,
            resourceGraph,
            resourceFactory,
            constraintProviders,
            loggerFactory)
        {
            _managementDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
            _busClient = busClient;
#pragma warning disable 612 // Method is obsolete
            _resourceDefinitionAccessor = resourceFactory.GetResourceDefinitionAccessor();
#pragma warning restore 612
        }

        private Guid CorrelationId
        {
            get
            {
                if (_httpContextAccessor
                    .HttpContext == null)
                {
                    throw new UnauthorizedAccessException();
                }

                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(
                    "x-correlation-id",
                    out var correlationId);

                var canParse = Guid.TryParse(
                    correlationId,
                    out var parsed);

                if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
                {
                    return NewId.NextGuid();
                }

                return parsed;
            }
        }

        protected override IQueryable<EventInstance> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!_httpContextAccessor.HttpContext.User.FindAll("eoid")
                .Any() && !_httpContextAccessor.IsAdmin())
            {
                return base.GetAll()
                    .Where(x => x.IsPublished);
            }

            return base.GetAll();
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var ei = await _managementDbContext.Set<EventInstance>()
                .Include(x => x.EventOrganiser)
                .FirstAsync(
                    x => x.Id == id,
                    cancellationToken);

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(ei.EventOrganiserId)
                           || _httpContextAccessor.OwnsResource(id);

            if (canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            await _resourceDefinitionAccessor.OnWritingAsync(
                ei,
                OperationKind.DeleteResource,
                cancellationToken);

            ei.Deleted = true;

            await _resourceDefinitionAccessor.OnWriteSucceededAsync(
                ei,
                OperationKind.DeleteResource,
                cancellationToken);

            await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task UpdateAsync(
            EventInstance resourceFromRequest,
            EventInstance resourceFromDatabase,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest);

            if (!canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            await ProcessFeaturesAsync(resourceFromRequest);

            var command = _httpContextAccessor.HttpContext?.Request.Headers["Command-Type"]
                .FirstOrDefault();

            if (command != null)
            {
                switch (command)
                {
                    case "PublishEventInstance":
                        await _busClient.Send<IPublishEventInstance>(
                            new PublishEventInstance
                            {
                                SourceId = resourceFromRequest.Id,
                                Published = resourceFromRequest.IsPublished,
                                Slug = resourceFromRequest.Slug,
                                CorrelationId = CorrelationId
                            });
                        break;
                    case "UnPublishEventInstance":
                        await _busClient.Send<IUnPublishEventInstance>(
                            new UnPublishEventInstance
                            {
                                SourceId = resourceFromRequest.Id,
                                Published = resourceFromRequest.IsPublished,
                                Slug = resourceFromRequest.Slug,
                                CorrelationId = CorrelationId
                            });
                        break;
                }
            }

            await base.UpdateAsync(
                resourceFromRequest,
                resourceFromDatabase,
                cancellationToken);
        }

        public override async Task CreateAsync(
            EventInstance resourceFromRequest,
            EventInstance resourceForDatabase,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest);

            if (!canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            resourceFromRequest.Id = NewId.NextGuid();

            // publishing is explicit. 
            resourceFromRequest.IsPublished = false;

            resourceFromRequest.OpeningTimes ??= new OpeningTimes();

            // default to the end customer paying these
            resourceFromRequest.PlatformFeePaidBy = 2;
            resourceFromRequest.PaymentPlatformFeePaidBy = 2;

            await ProcessFeaturesAsync(resourceFromRequest);

            resourceFromRequest.BusinessTypeId = resourceFromRequest.BusinessTypeId;

            try
            {
                await base.CreateAsync(
                    resourceFromRequest,
                    resourceForDatabase,
                    cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            await _busClient.Publish<IEventInstanceCreated>(
                new EventInstanceCreated
                {
                    SourceId = resourceFromRequest.Id,
                    Owner = new Owner
                    {
                        Id = resourceFromRequest.OwnerId.GetValueOrDefault()
                    },
                    EventSeriesId = resourceFromRequest.EventSeriesId,
                    Name = resourceFromRequest.Name,
                    Description = resourceFromRequest.Description,
                    Location = resourceFromRequest.Location,
                    PostCode = resourceFromRequest.PostCode,
                    DeliveryRadiusMiles = resourceFromRequest.DeliveryRadiusMiles,
                    Slug = resourceFromRequest.Slug,
                    Tagline = resourceFromRequest.Tagline,
                    Category = resourceFromRequest.Category,
                    EventOrganiserId = resourceFromRequest.EventOrganiserId,
                    CorrelationId = CorrelationId
                });
        }

        public async Task ProcessFeaturesAsync(
            EventInstance instance)
        {
            var businessTypeId = Guid.Empty;

            if (instance.BusinessTypeId.HasValue && instance.BusinessTypeId != Guid.Empty)
            {
                businessTypeId = instance.BusinessTypeId.GetValueOrDefault();
            }
            else if (instance.BusinessType != null && instance.BusinessType.Id != Guid.Empty)
            {
                businessTypeId = instance.BusinessType.Id;
            }
            else
            {
                var other = _managementDbContext.Set<BusinessType>()
                    .First(x => x.NormalizedName == "OTHER");
                instance.BusinessTypeId = other.Id;
                businessTypeId = other.Id;
            }

            instance.EventInstanceFeatures.Clear();

            var businessType = await _managementDbContext
                .Set<BusinessTypeFeatureTemplate>()
                .Include(x => x.BusinessTypeFeatureTemplateFeatures)
                .ThenInclude(x => x.Feature)
                .FirstOrDefaultAsync(x => x.BusinessTypeId == businessTypeId);

            foreach (var feature in businessType.BusinessTypeFeatureTemplateFeatures.Select(x => x.Feature))
                instance.EventInstanceFeatures.Add(
                    new EventInstanceFeature
                    {
                        EventInstance = instance,
                        Feature = feature
                    });
        }
    }
}