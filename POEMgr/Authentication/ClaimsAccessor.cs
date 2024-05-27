using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication
{
    public class ClaimsAccessor : IClaimsAccessor
    {
        private const string NAME = "name";
        private const string PREFERREDUSERNAME = "preferred_username";
        private const string OID = "oid";
        private const string ONJECTID = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        protected IPrincipalAccessor PrincipalAccessor { get; }

        public ClaimsAccessor(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor = principalAccessor;
        }

        public string UserId
        {
            get
            {
                string userId = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == OID)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    userId = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ONJECTID)?.Value;
                }

                return userId;
            }
        }
        public string Email
        {
            get
            {
                string roleIds = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == PREFERREDUSERNAME)?.Value;
                if (string.IsNullOrWhiteSpace(roleIds))
                {
                    return string.Empty;
                }

                return roleIds;
            }
        }

        public string DisplayName
        {
            get
            {
                string displayName = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == NAME)?.Value;
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == PREFERREDUSERNAME)?.Value;
                }

                return displayName;
            }
        }

        public string RoleName
        {
            get
            {
                string roleName = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return roleName;
            }
        }
    }

    public interface IClaimsAccessor
    {
        string UserId { get; }
        string DisplayName { get; }
        string Email { get; }
        string RoleName { get; }
    }
}
