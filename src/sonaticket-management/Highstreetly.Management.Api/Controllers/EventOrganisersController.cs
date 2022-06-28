using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class EventOrganisersController : JsonApiController<EventOrganiser, Guid>
    {
        public EventOrganisersController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<EventOrganiser, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}