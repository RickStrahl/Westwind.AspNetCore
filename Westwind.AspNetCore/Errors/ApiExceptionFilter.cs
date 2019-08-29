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
        private readonly bool _dontProcess;

        public ApiExceptionFilterAttribute(bool dontProcess = false)
        {
            _dontProcess = dontProcess;
        }
        public override void OnException(ExceptionContext context)
        {
            if (_dontProcess)
            {
                base.OnException(context);
                return;                
            }

            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;

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
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg);
                apiError.detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }

#if DEBUG
            // Get Code Line 
            if (!string.IsNullOrEmpty(apiError.detail))
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
                            apiError.code = lines[line - 1].Trim();
                        }
                    }
                }
                catch { }
            }
#endif

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }

}

