using System;
using System.Globalization;
using System.IO;
using System.Threading;
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
    /// <param name="context">HttpContext instance</param>
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

    /// <summary>
    /// Resolves of a virtual Url to a fully qualified Url.
    ///
    /// * ~/ ~ as base path
    /// * / as base path
    /// * https:// http:// return as is
    /// * Any relative path: returned as is
    /// * Empty or null: returned as is
    /// </summary>
    /// <returns>Updated path</returns>
    public static string ResolveUrl(this HttpContext context, string url)
    {
        if (string.IsNullOrEmpty(url) ||
            url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) )
            return url;

        var basepath = context.Request.GetSiteBaseUrl();

        if (url.StartsWith("~/"))
            url = basepath + url.Substring(2);
        else if (url.StartsWith("~") || url.StartsWith("/"))
            url = basepath + url.Substring(1);        

        // no leading path, ./ or ../
        // any relative Urls we can't do anything with
        // so return them as is and hope for the best

        return url;
    }

    /// <summary>
    /// Sets the culture and UI culture to a specific culture. Allows overriding of currency
    /// and optionally disallows setting the UI culture.
    /// 
    /// You can also limit the locales that are allowed in order to minimize
    /// resource access for locales that aren't implemented at all.
    /// </summary>
    /// <param name="culture">
    /// 2 or 5 letter ietf string code for the Culture to set. 
    /// Examples: en-US or en</param>
    /// <param name="uiCulture">ietf string code for UiCulture to set</param>
    /// <param name="currencySymbol">Override the currency symbol on the culture</param>
    /// <param name="setUiCulture">
    /// if uiCulture is not set but setUiCulture is true 
    /// it's set to the same as main culture
    /// </param>
    /// <param name="allowedLocales">
    /// Names of 2 or 5 letter ietf locale codes you want to allow
    /// separated by commas. If two letter codes are used any
    /// specific version (ie. en-US, en-GB for en) are accepted.
    /// Any other locales revert to the machine's default locale.
    /// Useful reducing overhead in looking up resource sets that
    /// don't exist and using unsupported culture settings .
    /// Example: de,fr,it,en-US
    /// </param>
    public static void SetUserLocale(this HttpContext httpContext, string culture = null,
        string uiCulture = null,
        string currencySymbol = null,
        bool setUiCulture = true,
        string allowedLocales = null)
    {
        // Use browser detection in ASP.NET
        if (string.IsNullOrEmpty(culture) && httpContext != null)
        {
            var request = httpContext.Request;

            
            // if no user lang leave existing but make writable
            if (request.Headers.AcceptLanguage.Count < 1)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                if (setUiCulture)
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Clone() as CultureInfo;

                return;
            }

            culture = request.Headers.AcceptLanguage[0];
        }
        else
            culture = culture.ToLower();

        if (!string.IsNullOrEmpty(uiCulture))
            setUiCulture = true;

        if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(allowedLocales))
        {
            allowedLocales = "," + allowedLocales.ToLower() + ",";
            if (!allowedLocales.Contains("," + culture + ","))
            {
                int i = culture.IndexOf('-');
                if (i > 0)
                {
                    if (!allowedLocales.Contains("," + culture.Substring(0, i) + ","))
                    {
                        // Always create writable CultureInfo
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                        if (setUiCulture)
                            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Clone() as CultureInfo;

                        return;
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(culture))
            culture = CultureInfo.InstalledUICulture.IetfLanguageTag;

        if (string.IsNullOrEmpty(uiCulture))
            uiCulture = culture;

        try
        {
            CultureInfo Culture = new CultureInfo(culture);

            if (currencySymbol != null && currencySymbol != "")
                Culture.NumberFormat.CurrencySymbol = currencySymbol;

            Thread.CurrentThread.CurrentCulture = Culture;

            if (setUiCulture)
            {
                var UICulture = new CultureInfo(uiCulture);
                Thread.CurrentThread.CurrentUICulture = UICulture;
            }
        }
        catch { }
    }

    
}
