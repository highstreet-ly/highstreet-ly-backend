using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{

    [Route("api/v1/register")]
    public class RegisterController : JsonApiController<Register, string>
    {
        public RegisterController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            ICreateService<Register, string> resourceService,
            IGetAllService<Register, string> getAllService)
            : base(options, loggerFactory, create: resourceService, getAll: getAllService)
        { }
    }
}