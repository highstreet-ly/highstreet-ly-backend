using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class TicketTypeConfigurationsController : JsonApiController<TicketTypeConfiguration, Guid>
    {
        public TicketTypeConfigurationsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<TicketTypeConfiguration, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}