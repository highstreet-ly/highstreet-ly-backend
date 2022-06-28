using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
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
    public class TicketTypeConfigurationService : JsonApiResourceService<TicketTypeConfiguration, Guid>
    {
        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ManagementDbContext _managementDbContext;
        private readonly IBusClient _busClient;
        private readonly IMapper _mapper;

        public TicketTypeConfigurationService(
            IResourceRepositoryAccessor repositoryAccessor,
            IQueryLayerComposer queryLayerComposer,
            IPaginationContext paginationContext,
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IJsonApiRequest request,
            IResourceChangeTracker<TicketTypeConfiguration> resourceChangeTracker,
            IResourceHookExecutorFacade hookExecutor,
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor,
            ManagementDbContext managementDbContext,
            IBusClient busClient,
            IMapper mapper)
            : base(repositoryAccessor, queryLayerComposer, paginationContext, options, loggerFactory, request,
                resourceChangeTracker, hookExecutor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _managementDbContext = managementDbContext;
            _busClient = busClient;
            _mapper = mapper;
        }

        public override async Task<TicketTypeConfiguration> CreateAsync(TicketTypeConfiguration resource,
            CancellationToken cancellationToken)
        {
            var @event = _managementDbContext
                .EventInstances
                .First(x => x.Id == resource.EventInstanceId);

            if (@event == null)
            {
                throw new Exception();
            }
            
            resource.AvailableQuantity = resource.Quantity;
            resource.EventInstanceId = @event.Id;
            resource.Id = NewId.NextGuid();

            // we are implicitly publishing products on creation now
            resource.IsPublished = true;

            await base.CreateAsync(resource, cancellationToken);

            await _busClient.Publish<ITicketTypeCreated>(new TicketTypeCreated
            {
                EventInstanceId = @event.Id,
                SourceId = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                Price = resource.Price,
                Quantity = resource.Quantity ?? 0,
                MainImageId = resource.MainImageId,
                Tags = resource.Tags,
                IsPublished = resource.IsPublished,
                CorrelationId = CorrelationId
            });

            return resource;
        }

        public override async Task<TicketTypeConfiguration> UpdateAsync(Guid id, TicketTypeConfiguration resource,
            CancellationToken cancellationToken)
        {
            var @event = _managementDbContext
                .EventInstances
                .First(x => x.Id == resource.EventInstanceId);

            if (@event == null)
            {
                throw new Exception();
            }
            
            var existing = await GetAsync(resource.Id, cancellationToken);

            var updatePrice = existing.Price != resource.Price;

            await base.UpdateAsync(id, resource, cancellationToken);

            await _busClient.Publish<ITicketTypeUpdated>(new TicketTypeUpdated
            {
                SourceId = resource.Id,
                EventInstanceId = resource.EventInstanceId,
                Name = resource.Name,
                Description = resource.Description,
                Price = resource.Price,
                Quantity = resource.Quantity ?? 0,
                FreeTier = resource.FreeTier,
                MainImageId = resource.MainImageId,
                Tags = resource.Tags,
                IsPublished = resource.IsPublished,
                Group = resource.Group,
                CorrelationId = CorrelationId,
                UpdatePrice = updatePrice,
                CommandSource = nameof(TicketTypeConfigurationService)
            });

            var command = _httpContextAccessor.HttpContext.Request.Headers["Command-Type"].FirstOrDefault();

            if (command != null)
            {
                switch (command)
                {
                    case "TicketTypePublished":
                        await _busClient.Publish<ITicketTypePublished>(new TicketTypePublished
                        {
                            EventInstanceId = resource.EventInstanceId,
                            SourceId = resource.Id,
                            CorrelationId = CorrelationId,
                        });
                        break;
                    case "TicketTypeUnpublished":
                        await _busClient.Publish<ITicketTypeUnpublished>(new TicketTypeUnpublished
                        {
                            EventInstanceId = resource.EventInstanceId,
                            SourceId = resource.Id,
                            CorrelationId = CorrelationId,
                        });
                        break;

                    case "AddStockQuantity":
                        await _busClient.Send<IAddTicketTypes>(
                            new AddTicketTypes
                            {
                                EventInstanceId = resource.EventInstanceId,
                                TicketType = resource.Id,
                                Quantity = resource.AddQuantity ?? 0,
                                CorrelationId = CorrelationId
                            });
                        break;
                }
            }

            return resource;
        }


        private Guid CorrelationId
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