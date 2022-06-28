using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class ImagesController : JsonApiController<Image, Guid>
    {
        public ImagesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<Image, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}