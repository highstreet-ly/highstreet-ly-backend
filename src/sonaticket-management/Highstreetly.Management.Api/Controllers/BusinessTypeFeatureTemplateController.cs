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
    public class BusinessTypeFeatureTemplateController : JsonApiController<BusinessTypeFeatureTemplate, Guid>
    {
        public BusinessTypeFeatureTemplateController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<BusinessTypeFeatureTemplate, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}