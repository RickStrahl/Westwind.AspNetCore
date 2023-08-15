using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Extensions;

public static class HttpContextExtensions
{
    static string WebRootPath { get; set; }
    static string ContentRootPath { get; set; }


    /// <summary>
    /// Maps a virtual or relative path to a physical path in a Web site,
    /// using the WebRootPath as the base path (ie. the `wwwroot` folder)
    /// </summary>
    /// <param name="request">HttpRequest instance</param>
    /// <param name="relativePath">Site relative path using either `~/` or `/` as indicating root</param>
    /// <param name="host">Optional - IHostingEnvironment instance. If not passed retrieved from RequestServices DI</param>
    /// <param name="basePath">Optional - Optional physical base path. By default host.WebRootPath</param>
    /// <param name="useAppBasePath">Optional - if true returns the launch folder rather than the wwwroot folder</param>
    /// <returns>physical path of the relative path</returns>
    public static string MapPath(this HttpContext context, string relativePath = null, IWebHostEnvironment host = null, string basePath = null, bool useAppBasePath = false)
    {
        if (string.IsNullOrEmpty(relativePath))
            relativePath = "/";

        // Ignore absolute paths
        if (relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            return relativePath;


        if (string.IsNullOrEmpty(basePath))
        {
            if(string.IsNullOrEmpty(WebRootPath) || string.IsNullOrEmpty(ContentRootPath))
            {
                host ??= context.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
                WebRootPath = host.WebRootPath;
                ContentRootPath = host.ContentRootPath;
            }
            basePath = useAppBasePath ? ContentRootPath.TrimEnd('/', '\\') : WebRootPath;
        }


        relativePath = relativePath.TrimStart('~', '/', '\\');

        string path = Path.Combine(basePath, relativePath);

        string slash = Path.DirectorySeparatorChar.ToString();
        return path
            .Replace("/", slash)
            .Replace("\\", slash)
            .Replace(slash + slash, slash);
    }
}
