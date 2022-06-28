using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   
    public class ClaimsController : JsonApiController<Claim, int>
    {
        public ClaimsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<Claim, int> resourceService)
            : base(options, loggerFactory, resourceService)
        { }
    }
}