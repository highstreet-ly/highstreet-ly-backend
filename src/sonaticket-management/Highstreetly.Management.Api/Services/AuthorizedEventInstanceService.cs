// using System;
// using System.Data;
// using System.Linq;
// using System.Net;
// using System.Threading;
// using System.Threading.Tasks;
// using Highstreetly.Infrastructure.Commands;
// using Highstreetly.Infrastructure.Events;
// using Highstreetly.Infrastructure.MessageDtos;
// using Highstreetly.Infrastructure.Messaging;
// using Highstreetly.Management.Resources;
// using JsonApiDotNetCore.Configuration;
// using JsonApiDotNetCore.Errors;
// using JsonApiDotNetCore.Hooks;
// using JsonApiDotNetCore.Middleware;
// using JsonApiDotNetCore.Queries;
// using JsonApiDotNetCore.Repositories;
// using JsonApiDotNetCore.Resources;
// using JsonApiDotNetCore.Serialization.Objects;
// using JsonApiDotNetCore.Services;
// using MassTransit;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace Highstreetly.Management.Api.Services
// {
//     public class AuthorizedEventInstanceService : JsonApiResourceService<EventInstance, Guid>
//     {
//         private readonly ManagementDbContext _managementDbContext;
//         private readonly IAuthorizationService _authService;
//         private readonly IHttpContextAccessor _httpContextAccessor;
//         private readonly IBusClient _busClient;
//
//         public AuthorizedEventInstanceService(
//             IResourceRepositoryAccessor repositoryAccessor,
//             IQueryLayerComposer queryLayerComposer,
//             IPaginationContext paginationContext,
//             IJsonApiOptions options,
//             ILoggerFactory loggerFactory,
//             IJsonApiRequest request,
//             IResourceChangeTracker<EventInstance> resourceChangeTracker,
//             IResourceHookExecutorFacade hookExecutor,
//             ManagementDbContext managementDbContext,
//             IAuthorizationService authService,
//             IHttpContextAccessor httpContextAccessor,
//             IBusClient busClient)
//             : base(repositoryAccessor, queryLayerComposer, paginationContext, options, loggerFactory, request,
//                 resourceChangeTracker, hookExecutor)
//         {
//             _managementDbContext = managementDbContext;
//             _authService = authService;
//             _httpContextAccessor = httpContextAccessor;
//             _busClient = busClient;
//         }
//
//         public override async Task<EventInstance> CreateAsync(EventInstance resource,
//             CancellationToken cancellationToken)
//         {
//             var evtOrgModel =  _managementDbContext.EventOrganisers.FirstOrDefault(x => x.Id == resource.EventOrganiserId);
//
//             if (evtOrgModel == null)
//             {
//                 var error = new Error(HttpStatusCode.NotFound)
//                 {
//                     Detail = "There is no event organiser with that ID"
//                 };
//                 throw new JsonApiException(error);
//             }
//
//             string id = null;
//
//             if (_httpContextAccessor.HttpContext.User != null &&
//                 _httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == "sub"))
//             {
//                 id = _httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "sub").Value;
//             }
//             else
//             {
//                 id = resource.OwnerId.ToString();
//             }
//
//             resource.OwnerId = Guid.Parse(id);
//
//             resource = await CreateEvent(resource);
//
//             await base.CreateAsync(resource, cancellationToken);
//
//             return resource;
//         }
//
//         public override async Task<EventInstance> UpdateAsync(Guid id, EventInstance resource, CancellationToken cancellationToken)
//         {
//             await base.UpdateAsync(id, resource, cancellationToken);
//
//             var command = _httpContextAccessor.HttpContext.Request.Headers["Command-Type"].FirstOrDefault();
//
//             if (command != null)
//             {
//                 switch (command)
//                 {
//                     case "PublishEventInstance":
//                         await _busClient.Send<IPublishEventInstance>(new PublishEventInstance
//                         {
//                             SourceId = resource.Id,
//                             Published = resource.IsPublished,
//                             Slug = resource.Slug,
//                             CorrelationId = CorrelationId
//                         });
//                         break;
//                     case "UnPublishEventInstance":
//                         await _busClient.Send<IUnPublishEventInstance>(new UnPublishEventInstance
//                         {
//                             SourceId = resource.Id,
//                             Published = resource.IsPublished,
//                             Slug = resource.Slug,
//                             CorrelationId = CorrelationId
//                         });
//                         break;
//                 }
//             }
//
//             return resource;
//         }
//
//         public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
//         {
//             // only allow delete if there are no active orders against the IE
//
//             var orders = await _managementDbContext.Orders.CountAsync(x => x.EventInstanceId == id, cancellationToken);
//
//             if (orders > 0)
//             {
//                 var error = new Error(HttpStatusCode.MethodNotAllowed)
//                 {
//                     Detail = "You cannot delete an event that has active orders"
//                 };
//                 throw new JsonApiException(error);
//             }
//
//             var toDelete =  _managementDbContext.EventInstances.FirstOrDefault(x => x.Id == id);
//             toDelete.Deleted = true;
//
//             await _managementDbContext.SaveChangesAsync(cancellationToken);
//         }
//
//         private async Task<EventInstance> CreateEvent(EventInstance ticketingEvent)
//         {
//             var existingSlug =
//                 _managementDbContext.EventInstances
//                     .Where(c => c.Slug == ticketingEvent.Slug)
//                     .Select(c => c.Slug)
//                     .Any();
//
//             if (existingSlug)
//             {
//                 throw new DuplicateNameException("The chosen ticketingEvent slug is already taken.");
//             }
//
//             // Conference publishing is explicit. 
//             if (ticketingEvent.IsPublished)
//             {
//                 ticketingEvent.IsPublished = false;
//             }
//
//             if (ticketingEvent.OpeningTimes == null)
//             {
//                 ticketingEvent.OpeningTimes = new OpeningTimes();
//             }
//
//             await _busClient.Publish<IEventInstanceCreated>(new EventInstanceCreated
//             {
//                 SourceId = ticketingEvent.Id,
//                 Owner = new Owner
//                 {
//                     Id = ticketingEvent.OwnerId.GetValueOrDefault()
//                 },
//                 EventSeriesId = ticketingEvent.EventSeriesId,
//                 Name = ticketingEvent.Name,
//                 Description = ticketingEvent.Description,
//                 Location = ticketingEvent.Location,
//                 PostCode = ticketingEvent.PostCode,
//                 DeliveryRadiusMiles = ticketingEvent.DeliveryRadiusMiles,
//                 Slug = ticketingEvent.Slug,
//                 Tagline = ticketingEvent.Tagline,
//                 Category = ticketingEvent.Category,
//                 EventOrganiserId = ticketingEvent.EventOrganiserId,
//                 CorrelationId = CorrelationId,
//
//             });
//
//             return ticketingEvent;
//         }
//
//         private Guid CorrelationId
//         {
//             get
//             {
//                 _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
//
//                 var canParse = Guid.TryParse(correlationId, out var parsed);
//
//                 if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
//                 {
//                     return NewId.NextGuid();
//                 }
//
//                 return parsed;
//             }
//         }
//     }
// }