using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Westwind.AspNetCore
{
    /// <summary>
    /// Helper class that consolidates access to Claims and Authentication
    /// features more easily. Subclass this class and add specific properties
    /// with Getters to retrieve data out of the user claims, roles
    /// and settings.
    /// </summary>
    public class AppUserBase
    {
        public AppUserBase(ClaimsPrincipal user)
        {
            User = user;
        }

        protected ClaimsPrincipal User { get; set; }

        public bool IsAuthenticated()
        {
            if (User == null || User.Identity == null)
                return false;

            return User.Identity.IsAuthenticated;
        }

        public bool IsEmpty()
        {
            return User == null || User.Identity == null;
        }

        public string GetClaim(string claimName)
        {
            return User.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
        }

        public bool HasRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}
