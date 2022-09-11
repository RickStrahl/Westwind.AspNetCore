using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Middleware
{

    /// <summary>
    /// Middleware to inject or remove HTTP headers on every request.
    /// </summary>
    public class CustomHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomHeadersToAddAndRemove _headers;


        public CustomHeadersMiddleware(RequestDelegate next, CustomHeadersToAddAndRemove headers)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _headers = headers;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!CheckforPrimaryRequests(context))
                return;

            foreach (var headerValuePair in _headers.HeadersToAdd)
            {
                context.Response.Headers[headerValuePair.Key] = headerValuePair.Value;
            }
            foreach (var header in _headers.HeadersToRemove)
            {
                context.Response.Headers.Remove(header);
            }

            await _next(context);
        }

        /// <summary>
        /// Checks for specific mime types to add/remove headers on
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if request should continue processing</returns>
        private bool CheckforPrimaryRequests(HttpContext context)
        {
            if (!_headers.PrimaryRequestsOnly)
                return true;

            var ct = context.Response.ContentType;

            foreach (var mt in _headers.PrimaryRequestMimeTypes)
            {
                if (ct.StartsWith(mt, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class CustomHeadersToAddAndRemove
    {
        public Dictionary<string, string> HeadersToAdd = new();
        public HashSet<string> HeadersToRemove = new();

        public bool PrimaryRequestsOnly { get; set;  }= false;

        public List<string> PrimaryRequestMimeTypes { get; set; } = new List<string>()
        {
            "text/html",
            "application/json",
            "text/xml",
            "application/xml"
        };

    }

    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Enable the Customer Headers middleware and specify the headers to add and remove.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="addHeadersAction">
        /// Action to allow you to specify the headers to add and remove.
        ///
        /// Example: (opt) =>  opt.HeadersToAdd.Add("header","value"); opt.HeadersToRemove.Add("header");</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder builder, Action<CustomHeadersToAddAndRemove> addHeadersAction)
        {
            var headers = new CustomHeadersToAddAndRemove();
            addHeadersAction?.Invoke(headers);

            builder.UseMiddleware<CustomHeadersMiddleware>(headers);
            return builder;
        }
    }
}
