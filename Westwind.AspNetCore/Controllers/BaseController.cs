using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Security;


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
        public UserState UserState;

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
            ViewBag.ErrorDisplay = ErrorDisplay;

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                UserState = UserState.CreateFromUserClaims<TUserState>(HttpContext) as TUserState;
            if (UserState == null)
                UserState = new UserState();
        }


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
        }


        ///// <summary>
        ///// Displays a self contained error page without redirecting.
        ///// Depends on ErrorController.ShowError() to exist
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="message"></param>
        ///// <param name="redirectTo"></param>
        ///// <returns></returns>
        //protected internal ActionResult DisplayErrorPage(string title, string message, string redirectTo = null, bool isHtml = true)
        //{
        //    ErrorController controller = new ErrorController();
        //    controller.InitializeExplicit(ControllerContext.RequestContext);
        //    return controller.ShowError(title, message, redirectTo, isHtml);
        //}


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
            return Json(new ApiError(errorMessage), new JsonSerializerSettings() {Formatting = Formatting.Indented});
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
            return Json(cb,new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

    }

}
