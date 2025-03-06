using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Extensions
{
    public static class HttpResponseExtensions
    {

        /// <summary>
        /// Adds a Meta Refresh header to the response that causes the browser to
        /// show the current page and then after specified seconds navigate to the
        /// url specified.
        ///
        /// The Url must be fully qualified or relative to the active page.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="url"></param>
        /// <param name="seconds"></param>
        public static void AddMetaRefreshTagHeader(this HttpResponse response, string url, int seconds = 5)
        {
            url = response.HttpContext.ResolveUrl(url);
            response.Headers.Append("Refresh", $"{seconds};url={url}");
        }

    }
}
