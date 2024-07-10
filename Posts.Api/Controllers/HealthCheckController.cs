using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Posts.Api.Controllers
{
    [Route("/")]
    [AllowAnonymous]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            return Ok(); // TODO - Implement health checks
        }
    }
}