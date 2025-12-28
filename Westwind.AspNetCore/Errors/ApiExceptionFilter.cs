using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Westwind.Utilities;


namespace Westwind.AspNetCore.Errors
{
    /// <inheritdoc />
    /// <summary>
    /// Unhandled Exception filter attribute for API controllers.
    /// Fires back a common JSON response of type ApiErrorResponse
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Global static value that allows you to output detailed
        /// error information. Set this during startup in the
        /// `IsDevelopment()` startup block.
        /// </summary>
        public static bool ShowExceptionDetail { get; set; } = false;

        /// <summary>
        /// Global static interception operation you can use to handle exceptions.
        ///
        /// Return true to indicate you handled the exception and default processing
        /// should not proceed.
        /// </summary>
        public static Func<ExceptionContext, bool> OnBeginExceptionProcessed { get; set; }

        /// <summary>
        /// Global static interception operation you can use to handle exceptions.
        /// This handler is called at the end of the error processing and contains
        /// the JSON Error context.Result that was generated and returned.
        /// </summary>
        public static Action<ExceptionContext> OnEndExceptionProcessed { get; set; }


        /// <summary>
        ///
        /// </summary>
        private bool _dontProcess { get; }
                    
                    
        /// <summary>
        /// Handles exceptions for API requests and displays an error response
        /// </summary>
        /// <param name="dontProcess">Optional - can be used to bypass the exception handling temporarily. Useful for debugging at times. Mirrors base exception filter.</param>
        public ApiExceptionFilterAttribute(bool dontProcess = false)
        {
            _dontProcess = dontProcess;            
        }

        public override void OnException(ExceptionContext context)
        {
            // allow interception via static event handler
            // configure in startup code - useful for logging etc.
            if(OnBeginExceptionProcessed != null)
            {
                if (OnBeginExceptionProcessed.Invoke(context))
                    return;
            }              
            
            ApiError apiError = null;
            if (context.Exception is Westwind.AspNetCore.Errors.ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;               
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;
                apiError.errorCode = ex.ErrorCode;

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                apiError.detail = context.Exception.Message;
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                string msg, stack;

                // Unhandled errors
                if (!ShowExceptionDetail)
                {
                    msg = "An unhandled error occurred.";
                    stack = null;
                }
                else
                {
                    msg = context.Exception.GetBaseException().Message;
                    stack = context.Exception.StackTrace;
                }

                apiError = new ApiError(msg);
                apiError.detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }


            // Get Code Line
            if (ShowExceptionDetail && !string.IsNullOrEmpty(apiError.detail))
            {
                try
                {
                    var firstLine = StringUtils.GetLines(apiError.detail, 1)[0];

                    if (!string.IsNullOrEmpty(firstLine) && firstLine.Contains(".cs:line "))
                    {
                        firstLine = StringUtils.ExtractString(firstLine, " in ", "xxx", allowMissingEndDelimiter: true);

                        var tokens = firstLine.Split(new[] {":line "}, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length > 0 && System.IO.File.Exists(tokens[0]))
                        {
                            var line = int.Parse(tokens[1]);
                            var fc = File.ReadAllText(tokens[0]);
                            var lines = StringUtils.GetLines(fc);
                            apiError.source = lines[line - 1].Trim();
                        }
                    }
                }
                catch { }
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);

            OnEndExceptionProcessed?.Invoke(context) ;
        }
    }

}

