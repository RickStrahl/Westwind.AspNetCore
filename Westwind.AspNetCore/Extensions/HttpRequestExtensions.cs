using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Westwind.Utilities;
using Westwind.Web;

namespace Westwind.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        static string WebRootPath { get; set; }

        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <param name="inputStream">Optional - Pass in the stream to retrieve from. Other Request.Body</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null, Stream inputStream = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (inputStream == null)
                inputStream = request.Body;
            
            using (StreamReader reader = new StreamReader(inputStream, encoding))
                return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request, Stream inputStream = null)
        {
            if (inputStream == null)
                inputStream = request.Body;

            using (var ms = new MemoryStream(2048))
            {
                await inputStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

#if NETCORE2
        /// <summary>
        /// Maps a virtual or relative path to a physical path in a Web site
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativePath"></param>
        /// <param name="host">Optional - IHostingEnvironment instance. If not passed retrieved from RequestServices DI</param>
        /// <param name="basePath">Optional - Optional physical base path. By default host.WebRootPath</param>
        /// <returns></returns>
        public static string MapPath(this HttpRequest request, string relativePath, IHostingEnvironment host = null, string basePath = null)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (basePath == null)
            {
                if (string.IsNullOrEmpty(WebRootPath))
                {
                    if (host == null)
                        host =
                            request.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as
                                IHostingEnvironment;
                    WebRootPath = host.WebRootPath;

                }

                if (string.IsNullOrEmpty(relativePath))
                    return WebRootPath;

                basePath = WebRootPath;
            }

            relativePath = relativePath.TrimStart('~').TrimStart('/', '\\');

            if (relativePath.StartsWith("~"))
                relativePath = relativePath.TrimStart('~');

            string path = Path.Combine(basePath, relativePath);

            string slash = Path.DirectorySeparatorChar.ToString();
            return path
                .Replace("/", slash)
                .Replace("\\", slash)
                .Replace(slash + slash, slash);
        }
#else
        /// <summary>
        /// Maps a virtual or relative path to a physical path in a Web site
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativePath"></param>
        /// <param name="host">Optional - IHostingEnvironment instance. If not passed retrieved from RequestServices DI</param>
        /// <param name="basePath">Optional - Optional physical base path. By default host.WebRootPath</param>
        /// <returns></returns>
        public static string MapPath(this HttpRequest request, string relativePath, IWebHostEnvironment host = null, string basePath = null)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (basePath == null)
            {
                if (string.IsNullOrEmpty(WebRootPath))
                {
                    if (host == null)
                        host =
                            request.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as
                                IWebHostEnvironment;
                    WebRootPath = host.WebRootPath;

                }

                if (string.IsNullOrEmpty(relativePath))
                    return WebRootPath;

                basePath = WebRootPath;
            }

            relativePath = relativePath.TrimStart('~').TrimStart('/', '\\');

            if (relativePath.StartsWith("~"))
                relativePath = relativePath.TrimStart('~');

            string path = Path.Combine(basePath, relativePath);

            string slash = Path.DirectorySeparatorChar.ToString();
            return path
                .Replace("/", slash)
                .Replace("\\", slash)
                .Replace(slash + slash, slash);
        }
#endif

        /// <summary>
        /// Returns the absolute Url of the current request as a string.
        /// </summary>
        /// <param name="request"></param>
        public static string GetUrl(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }


        /// <summary>
        /// Returns a value based on a key against the Form, Query and Session collections.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="noSession">If true Session object is not check</param>
        /// <returns></returns>
        public static string Params(this HttpRequest request, string key)
        {
            string value = request.Form[key].FirstOrDefault();
            if (string.IsNullOrEmpty(value))
                value = request.Query[key].FirstOrDefault();

            return value;
        }

        /// <summary>
        /// Determines if the request is a local request where the local and remote IP addresses match
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);

                return IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
                return true;
            //if (req.Host.HasValue && req.Host.Value.StartsWith("localhost:") )
            //    return true;

            return false;
        }

        /// <summary>
        /// Checks to see if a given form variable exists in the request form collection.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="formVarName"></param>
        /// <returns></returns>
        public static bool IsFormVar(this HttpRequest req, string formVarName)
        {
            if (!req.HasFormContentType) return false;

            return req.Form[formVarName].Count > 0;
        }

        /// <summary>
        /// Determines whether request is a postback operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPostback(this HttpRequest request)
        {
            return request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                   request.Method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Unbinds form variable data to an instantiated object by matching form variable name to a property name
        /// and only updating properties that have matching form variables and leaving the rest alone.
        /// </summary>
        /// <remarks>Only updates immediate instance properties - does not handle nested objects or collections</remarks>
        /// <param name="request">ASP.NET HttpRequest object </param>
        /// <param name="targetObject">Existing object to update with Form data</param>
        /// <param name="propertyExceptions">Optional comma delimited list of properties that shouldn't be updated.</param>
        /// <param name="formvarPrefixes">Optional prefix to</param>
        /// <returns></returns>
        public static List<ValidationError> UnbindFormVarsToObject(this HttpRequest request, object targetObject, string propertyExceptions = null, string formvarPrefixes = null)
        {
            return FormVariableBinder.Unbind(request, targetObject, propertyExceptions, formvarPrefixes);
        }

        /// TODO: Create a generic way to retrieve the route dictionary
        //public static string GetRouteValue(this HttpRequest request, string routeKey)
        //{
        //      throw new NotImplementedException();
        //}
    }
}
