using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Security
{
    /// <summary>
    /// Helper class that consolidates access to Claims and Authentication
    /// features more easily. Subclass this class and add specific properties
    /// with Getters to retrieve data out of the user claims, roles
    /// and settings.
    /// </summary>
    /// <remarks>
    /// Create an application specific class that inherits from AppUserBase and
    /// uses specific claims retrieved as property values:
    /// 
    /// ```cs
    /// public class AppUser : AppUserBase
    /// {
    ///   public AppUser(ClaimsPrincipal user) : base(user) { }
    ///
    ///   public string Email => GetClaim("Username");
    ///   public string Fullname => GetClaim("Fullname");
    ///   public string UserId => GetClaim("UserId");
    ///   public bool IsAdmin => HasRole("Admin");
    /// }    
    /// ```
    /// retrieves a user from ClaimsPrincipal with your own implementation.
    /// 
    /// ```cs
    /// public static class ClaimsPrincipalExtensions
    /// {
    ///    public static AppUser GetAppUser(this ClaimsPrincipal user)
    ///    {
    ///            return new AppUser(user);
    ///    }
    /// }
    /// ```
    /// </remarks>
    public class AppUserBase
    {
        public AppUserBase(ClaimsPrincipal user)
        {
            User = user;
        }

        protected ClaimsPrincipal User { get; set; }

        public IEnumerable<Claim> Claims => User?.Claims;

        #region Claims Validation
        public virtual bool IsAuthenticated()
        {
            if (User == null || User.Identity == null)
                return false;

            return User.Identity.IsAuthenticated;
        }

        public virtual bool IsEmpty()
        {
            return User?.Identity == null;
        }

        public bool HasRole(string role)
        {
            return User.IsInRole(role);
        }
        #endregion

        #region Claims and Roles

        public string GetClaim(string claimName)
        {
            return User.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
        }

        public void AddClaim(string claimName, string value)
        {
            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(claimName, value));
        }

        public void AddOrUpdateClaim(string claimName, string value)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == claimName);

            if (claim != null && claim.Value != value)
                ((ClaimsIdentity)User.Identity).RemoveClaim(claim);

            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(claimName, value));
        }

        public void AddRole(string userRole)
        {
            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(ClaimTypes.Role, userRole));
        }

        #endregion

        #region Login Logout

        public async Task LoginUserAsync(HttpContext context)
        {
            // Set cookie and attach claims
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(User.Identity));
        }

        public void LoginUser(HttpContext context)
        {
            // Set cookie and attach claims
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(User.Identity));
        }

        public async Task LogoutUserAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public void LogoutUser(HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        #endregion

    }
}

// Create a class in your application that simulates this
//namespace System.Security.Claims
//{
//    public static class ClaimsPrincipalExtensions
//    {
//        public static MyAppUser GetAppUser(this ClaimsPrincipal user)
//        {
//            return new AppUser(user);
//        }
//    }
//}

