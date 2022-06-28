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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductCategoriesController : JsonApiController<ProductCategory, Guid>
    {
        public ProductCategoriesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<ProductCategory, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}