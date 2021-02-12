using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Security;


namespace Westwind.AspNetCore
{


    /// <summary>
    /// Base Controller implementation that holds the default UserState object
    /// and provides ApiException Handling, UserState initialization
    /// and JsonError functionality.
    ///
    /// Uses BaseController as its base class.
    /// </summary>
    public class BaseApiController : BaseApiController<UserState>
    {

    }

    /// <summary>
    /// Base Controller implementation that holds a custom UserState object
    /// and provides ApiException Handling, UserState initialization
    /// and JsonError functionality.
    ///
    /// Uses BaseController as its base class.
    /// </summary>
    [ApiExceptionFilter(false)]
    [UserStateBaseApiControllerFilter(false)]
    public class BaseApiController<TUserState> : BaseController<TUserState>
        where TUserState : UserState, new()
    {
        /// <summary>
        /// UserState instance that holds cached user data info that
        /// gets persisted into an authentication cookie or token for
        /// easy reuse without reloading a user record.
        ///
        /// Works with Auth Cookie or JWT token Authentication just ensure
        /// you write out UserState into claims when creating Cookie/Token
        /// via <seealso cref="UserState.ToString"/>
        /// </summary>
        public new TUserState UserState = new TUserState();


        /// <summary>
        /// Returns a Json error response to the client
        /// </summary>
        /// <param name="errorMessage">Message of the error to return</param>
        /// <param name="statusCode">Optional status code.</param>
        /// <returns></returns>
        public new JsonResult JsonError(string errorMessage, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            return new JsonResult(new ApiError(errorMessage), new JsonSerializerSettings() {Formatting = Formatting.Indented});
        }

        /// <summary>
        /// Returns a JSON error response to the client
        /// </summary>
        /// <param name="ex">Exception that generates the error message and info to return</param>
        /// <param name="statusCode">Optional status code</param>
        /// <returns></returns>
        public new JsonResult ReturnJsonError(Exception ex, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            var cb = new ApiError(ex.GetBaseException().Message);
#if DEBUG
            cb.detail = ex.StackTrace;
#endif
            return new JsonResult(cb,new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

    }

    /// <summary>
    /// Base Controller implementation that holds the default UserState object
    /// and provides ApiException Handling, UserState initialization
    /// and JsonError functionality.
    ///
    /// Uses Controller as its base class.
    /// </summary>
    public class BaseApiFromFullController : BaseApiFromFullController<UserState>
    {

    }


    /// <summary>
    /// Base Controller implementation that holds a custom UserState object
    /// and provides ApiException Handling, UserState initialization
    /// and JsonError functionality
    ///
    /// Uses Controller as its base class.
    /// </summary>
    [ApiExceptionFilter(false)]
    [UserStateBaseApiControllerFilter(false)]
    public class BaseApiFromFullController<TUserState> : Controller
        where TUserState : UserState, new()
    {
        /// <summary>
        /// UserState instance that holds cached user data info that
        /// gets persisted into an authentication cookie or token for
        /// easy reuse without reloading a user record.
        ///
        /// Works with Auth Cookie or JWT token Authentication just ensure
        /// you write out UserState into claims when creating Cookie/Token
        /// via <seealso cref="UserState.ToString"/>
        /// </summary>
        public TUserState UserState = new TUserState();


        /// <summary>
        /// Returns a Json error response to the client
        /// </summary>
        /// <param name="errorMessage">Message of the error to return</param>
        /// <param name="statusCode">Optional status code.</param>
        /// <returns></returns>
        public JsonResult JsonError(string errorMessage, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            return new JsonResult(new ApiError(errorMessage), new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }

        /// <summary>
        /// Returns a JSON error response to the client
        /// </summary>
        /// <param name="ex">Exception that generates the error message and info to return</param>
        /// <param name="statusCode">Optional status code</param>
        /// <returns></returns>
        public JsonResult ReturnJsonError(Exception ex, int statusCode = 500)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            var cb = new ApiError(ex.GetBaseException().Message);
#if DEBUG
            cb.detail = ex.StackTrace;
#endif
            return new JsonResult(cb, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

    }



    /// <summary>
    /// Filter that handles parsing UserState if it exists
    /// in the User claims. Basically looks
    /// </summary>
    public class UserStateBaseApiControllerFilterAttribute : ActionFilterAttribute
    {
        private bool DontProcess { get;  }

        public UserStateBaseApiControllerFilterAttribute(bool dontProcess = false)
        {
            DontProcess = dontProcess;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Initialize(context);
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Initialize the controller by setting up UserState and ErrorDisplay
        /// </summary>
        protected virtual void Initialize(ActionExecutingContext context)
        {
            if (!DontProcess)
                ParseUserState(context);
        }


        /// <summary>
        /// This method Parses user state from the User.Identity if the
        /// user is Authenticated. Otherwise the UserState object is
        /// left as an empty object.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ParseUserState(ActionExecutingContext context)
        {
            var controller = context.Controller as BaseApiController;
            if (controller == null)
                return;  // can't parse return without userstate

            var user = context.HttpContext.User;
            var userStateType = controller.UserState.GetType();

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                controller.UserState = UserState.CreateFromUserClaims(context.HttpContext, userStateType);
        }
    }

}
