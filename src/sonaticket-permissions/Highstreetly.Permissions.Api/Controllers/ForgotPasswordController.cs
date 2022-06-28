using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{
     //[Route("api/v1/forgot-password"), DisableRoutingConvention]
     public class ForgotPasswordController : JsonApiController<ForgotPassword, string>
    {
        public ForgotPasswordController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            ICreateService<ForgotPassword, string> resourceService)
            : base(options, loggerFactory, create: resourceService)
        { }
    }
}