using Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using System.Dynamic;

namespace POEMgr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimsAccessor claimsAccessor;

        public UsersController(IClaimsAccessor claimsAccessor , IUserService userRoleService)
        {
            this._userService = userRoleService;
            this.claimsAccessor = claimsAccessor;
        }

        [HttpGet]
        [Route("currentUser")]
        public async Task<IActionResult> User_getCurrentUser()
        {
            User_getCurrentUser_req user = new User_getCurrentUser_req();
            user.Name = claimsAccessor.DisplayName;
            user.Email = claimsAccessor.Email;
            user.UserId = claimsAccessor.UserId;
            return Ok(await _userService.User_getCurrentUser(user));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> User_add([FromBody] User_add_req p)
        {
            return Ok(await _userService.User_add(p));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest model)
        {
            return Ok(await _userService.GetUsers(model));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest model)
        {
            return Ok(await _userService.UpdateUser("", model));
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteUsers(List<string> ids)
        {
            return Ok(await _userService.DeleteUsers(ids));
        }

        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _userService.GetRoles());
        }
    }
}
