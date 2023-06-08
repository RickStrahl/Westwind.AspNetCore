using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Security;
using Westwind.Utilities;


namespace Westwind.AspNetCore
{


    /// <summary>
    /// Base Controller implementation that holds ViewState options,
    /// ErrorDisplay and UserState objects that are preinitialized
    /// </summary>
    public class AppBaseController : BaseController<UserState>
    {

    }

    public class BaseController<TUserState> : Controller
        where TUserState : UserState, new()
    {
        /// <summary>
        /// ErrorDisplay control that holds page level error information
        /// </summary>
        public ErrorDisplayModel ErrorDisplay = new ErrorDisplayModel();

        /// <summary>
        /// UserState instance that holds cached user data info that
        /// gets persisted into an authentication cookie or token for
        /// easy reuse without reloading a user record.
        ///
        /// Works with Auth Cookie or JWT token Authentication just ensure
        /// you write out UserState into claims when creating Cookie/Token
        /// via <seealso cref="UserState.ToString"/>
        /// </summary>
        public virtual TUserState UserState { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Initialize(context);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (UserStateWebSettings.Current.IsUserStateEnabled)
                PersistUserState(UserState);
        }

        /// <summary>
        /// Initialize the controller by setting up UserState and ErrorDisplay
        /// </summary>
        protected virtual void Initialize(ActionExecutingContext context)
        {
            ViewBag.ErrorDisplay = ErrorDisplay;
            if (UserStateWebSettings.Current.IsUserStateEnabled)
            {
                UserState = CreateUserState();
            }
        }

        #region View Models

        /// <summary>
        /// Creates or updates a ViewModel and adds values to some of the
        /// stock properties of the Controller.
        ///
        /// This default implementation initializes the ErrorDisplay and UserState
        /// objects after creation.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        protected virtual TViewModel CreateViewModel<TViewModel>()
            where TViewModel : class, new()
        {
            var model = new TViewModel();

            if (model is BaseViewModel)
            {
                BaseViewModel baseModel = model as BaseViewModel;
                baseModel.ErrorDisplay = ErrorDisplay;
            }

            return model;
        }

        /// <summary>
        /// Updates a ViewModel and adds values to some of the
        /// stock properties of the Controller.
        ///
        /// This default implementation initializes the ErrorDisplay and UserState
        /// objects after creation.
        /// </summary>
        protected virtual void InitializeViewModel(BaseViewModel model)
        {
            if (model == null)
                return;

            BaseViewModel baseModel = model as BaseViewModel;
            baseModel.ErrorDisplay = ErrorDisplay;

            if (baseModel is BaseViewModel<TUserState>)
            {
                var baseUserModel = baseModel as BaseViewModel<TUserState>;
                baseUserModel.UserState = UserState;
            }
        }

#endregion

        #region Json Error Results

        /// <summary>
        /// Returns a Json error response to the client
        /// </summary>
        /// <param name="errorMessage">Message of the error to return</param>
        /// <param name="statusCode">Optional status code.</param>
        /// <returns></returns>
        protected JsonResult JsonError(string errorMessage, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            return Json(new ApiError(errorMessage), new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }

        /// <summary>
        /// Returns a JSON error response to the client
        /// </summary>
        /// <param name="ex">Exception that generates the error message and info to return</param>
        /// <param name="statusCode">Optional status code</param>
        /// <returns></returns>
        protected JsonResult ReturnJsonError(Exception ex, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            var cb = new ApiError(ex.GetBaseException().Message);
#if DEBUG
            cb.detail = ex.StackTrace;
#endif
            return Json(cb, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        #endregion



        #region UserState Management

        /// <summary>
        /// Keep track of initial state in this Request, so we don't write out cookies
        /// when nothing has changed.
        /// </summary>
        protected string _initialAppUserState = null;

        /// <summary>
        /// Use this to create a UserState instance - typically call from `Initialize()` method of controller
        /// prior to action execution.
        /// </summary>
        /// <param name="mode">Mode using either Identity Claims or an Http Cookie to store data</param>
        /// <param name="cookieName">Name of the cookie to use if using cookies. Cookie is only used in CookieMode</param>
        protected TUserState CreateUserState()
        {

            var settings = UserStateWebSettings.Current;
            if (!settings.IsUserStateEnabled) return new TUserState();

            TUserState userState = null;

            string rawCookie = null;
            if (settings.PersistanceMode == UserStatePersistanceModes.Cookie)
            {
                rawCookie = HttpContext.Request.Cookies[settings.CookieName];
                if(!string.IsNullOrEmpty(rawCookie))
                    rawCookie = Encryption.DecryptString(rawCookie, UserStateWebSettings.Current.CookieEncryptionKey, true);
            }
            else
            {
                var httpUser = User.Identity as ClaimsIdentity;
                if (httpUser == null)
                {
                    userState = new TUserState();
                }
                rawCookie = httpUser.FindFirst("UserState")?.Value;
            }

            if (string.IsNullOrEmpty(rawCookie))
            {
                userState = new TUserState();
            }
            else
            {
                try
                {
                    _initialAppUserState = rawCookie;

                    userState =  Westwind.AspNetCore.Security.UserState.CreateFromString<TUserState>(_initialAppUserState);
                    if (userState == null)
                        userState = new TUserState();
                }
                catch
                {
                    userState = new TUserState();
                }
            }

            return userState;
        }


        /// <summary>
        /// Persists UserState in a cookie or as an Identity Claim so it can be picked up
        /// in subsequent requests.
        /// </summary>
        /// <param name="userState">UserState to save</param>
        /// <param name="mode"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        private async Task PersistUserState(UserState userState)
        {
            var settings = UserStateWebSettings.Current;
            if (!settings.IsUserStateEnabled) return;

            // Persist UserState in Cookie if state has changed
            var updatedUserState = userState.ToString();
            if (updatedUserState != _initialAppUserState)
            {
                string rawCookie = updatedUserState;

                // cookie has to be encrypted
                if (settings.PersistanceMode == UserStatePersistanceModes.Cookie)
                {
                    rawCookie = Encryption.EncryptString(updatedUserState, UserStateWebSettings.Current.CookieEncryptionKey, true);
                }
                

                if (settings.PersistanceMode == UserStatePersistanceModes.Cookie)
                {
                    HttpContext.Response.Cookies.Delete(settings.CookieName);

                    var cookieOptions = new CookieOptions { SameSite = SameSiteMode.Strict, HttpOnly = true };
                    if (settings.CookieTimeoutDays > 0)
                        cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(settings.CookieTimeoutDays);

                    HttpContext.Response.Cookies.Append(settings.CookieName, rawCookie, cookieOptions);
                }
                else
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userState.UserId));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userState.Name));
                    identity.AddClaim(new Claim("UserState", rawCookie));

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties { IsPersistent = true, AllowRefresh = true, };
                    if (settings.CookieTimeoutDays > 0)
                        authProperties.ExpiresUtc = DateTime.UtcNow.AddDays(settings.CookieTimeoutDays);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);
                }
            }
        }


        #endregion

    }


    /// <summary>
    /// Application wide configuration for UserState Web Settings
    /// for Cookie and Identity use.
    /// </summary>
    public class UserStateWebSettings
    {
        public static UserStateWebSettings Current
        {
            get
            {
                if (_current == null)
                    _current = new UserStateWebSettings();
                return _current;
            }
            set
            {
                _current = value;
            }
        }
        private static UserStateWebSettings _current;

        public bool IsUserStateEnabled { get; set; } = true;

        public UserStatePersistanceModes PersistanceMode { get; set; } = UserStatePersistanceModes.IdentityClaims;
        public string CookieName { get; set; } = "us_dt";
        public string CookieEncryptionKey { get; set; } = "5tG9s#4bx0-2*35dQWo98i9uU3a--";
        public int CookieTimeoutDays { get; set; } = 1;
    }


    public enum UserStatePersistanceModes
    {
        IdentityClaims,
        Cookie
    }
}
