using Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace POEMgr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class HomeController : Controller
    {
        private readonly IClaimsAccessor claimsAccessor;
        public HomeController(IClaimsAccessor claimsAccessor)
        {
            this.claimsAccessor = claimsAccessor;
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("Successfully");
        }

        [HttpGet]
        [Route("currentuser")]
        public IActionResult GetCurrentUser()
        {
            dynamic user = new ExpandoObject();
            user.Name = claimsAccessor.DisplayName;
            user.Email = claimsAccessor.Email;
            user.UserId = claimsAccessor.UserId;

            return Ok(user);
        }
    }
}
