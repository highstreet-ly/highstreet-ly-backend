using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductExtraGroupsController : JsonApiController<ProductExtraGroup, Guid>
    {
        public ProductExtraGroupsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<ProductExtraGroup, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}