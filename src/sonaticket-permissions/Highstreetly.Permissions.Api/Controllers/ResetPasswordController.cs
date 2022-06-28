using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{
    public class ResetPasswordController : JsonApiController<ResetPassword, string>
    {
        public ResetPasswordController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            ICreateService<ResetPassword, string> resourceService)
            : base(options, loggerFactory, create: resourceService)
        { }
    }
}