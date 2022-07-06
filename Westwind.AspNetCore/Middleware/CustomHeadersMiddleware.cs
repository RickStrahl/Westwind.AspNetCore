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
    }

    public class CustomHeadersToAddAndRemove
    {
        public Dictionary<string, string> HeadersToAdd = new();
        public HashSet<string> HeadersToRemove = new();
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
