using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class EventInstancesController : JsonApiController<EventInstance, Guid>
    {
        public EventInstancesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<EventInstance, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}