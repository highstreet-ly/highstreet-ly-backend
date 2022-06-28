using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{
    [Route("api/v1/confirm-email"), DisableRoutingConvention]
    public class ConfirmEmailController : BaseJsonApiController<ConfirmEmail, string>
    {
        public ConfirmEmailController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            ICreateService<ConfirmEmail, string> resourceService)
            : base(options, loggerFactory, create: resourceService)
        { }

        [HttpPost]
        public override Task<IActionResult> PostAsync(ConfirmEmail resource, CancellationToken cancellationToken)
        {
            return base.PostAsync(resource, cancellationToken);
        }

        [HttpGet]
        public override Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IActionResult)StatusCode(StatusCodes.Status400BadRequest));
        }
    }
}