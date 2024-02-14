using Microsoft.AspNetCore.Mvc;

namespace IviMessageServer.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello World");
        }
    }
}
