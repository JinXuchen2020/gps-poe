using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Authentication
{
    public class PrincipalAccessor : IPrincipalAccessor
    {
        public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public PrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }

    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}