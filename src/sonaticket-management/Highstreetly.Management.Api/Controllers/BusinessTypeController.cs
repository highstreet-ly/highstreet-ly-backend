using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class BusinessTypeController : JsonApiController<BusinessType, Guid>
    {
        public BusinessTypeController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<BusinessType, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}