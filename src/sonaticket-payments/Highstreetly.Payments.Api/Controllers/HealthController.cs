using Microsoft.AspNetCore.Mvc;

namespace Highstreetly.Payments.Api.Controllers
{
    [Route("/healthz")]
    [Route("/")]
    public class HealthController : Controller
    {
        public IActionResult GetAction()
        {
            return Ok();
        }
    }
}