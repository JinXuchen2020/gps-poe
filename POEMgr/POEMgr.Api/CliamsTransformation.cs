using Authentication;
using Microsoft.AspNetCore.Authentication;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using System.Security.Claims;
using POEMgr.Application;

namespace POEMgr.Api
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserService _userService;
        public ClaimsTransformation(IUserService userService) 
        {
            _userService = userService;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var clonePrincipal = principal.Clone();
            var newIdentity = clonePrincipal.Identity as ClaimsIdentity ?? new ClaimsIdentity();

            User_getCurrentUser_req user = new User_getCurrentUser_req();
            user.Name = principal.UserName();
            user.Email = principal.Email();
            user.UserId = principal.UserId();
            var result = await _userService.User_getCurrentUser(user);

            if (!newIdentity.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                newIdentity.AddClaim(new Claim(ClaimTypes.Role, result.Data.RoleName));
            }

            return clonePrincipal;
        }
    }
}
