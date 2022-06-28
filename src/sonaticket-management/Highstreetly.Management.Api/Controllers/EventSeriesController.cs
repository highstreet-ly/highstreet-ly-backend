using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class EventSeriesController : JsonApiController<EventSeries, Guid>
    {
        public EventSeriesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<EventSeries, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}