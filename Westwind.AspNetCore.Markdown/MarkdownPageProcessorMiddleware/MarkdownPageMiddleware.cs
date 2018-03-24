#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 
 *          http://www.west-wind.com/
 * 
 * Created: 3/25/2018
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Middleware that allows you to serve static Markdown files from disk
    /// and merge them using a configurable View template.
    /// </summary>
    public class MarkdownProcessorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MarkdownPageProcessorConfiguration _configuration;

        public MarkdownProcessorMiddleware(RequestDelegate next, 
                                               MarkdownPageProcessorConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path == null)
                return _next(context);

            bool hasExtension = !string.IsNullOrEmpty(System.IO.Path.GetExtension(path));
            bool hasMdExtension = path.EndsWith(".md");
            bool isRoot = path == "/";
            bool processAsMarkdown = false;

            // process any Markdown file that has .md extension explicitly
            foreach (var folder in _configuration.MarkdownProcessingFolders)
            {
                if (!path.ToLower().StartsWith(folder.RelativePath.ToLower()))
                    continue;

                if (context.Request.Path.Value.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
                    processAsMarkdown = true;
                // Root Url = everything is processed (not a good idea)
                else if (isRoot)
                {
                    if (folder.RelativePath != "/")
                        continue;

                    if (folder.ProcessExtensionlessUrls && !hasExtension || hasMdExtension && folder.ProcessMdFiles)
                        processAsMarkdown = true;
                }
                else if (path.ToLower().StartsWith(folder.RelativePath.ToLower()) && (folder.ProcessExtensionlessUrls && !hasExtension || hasMdExtension && folder.ProcessMdFiles))
                    processAsMarkdown = true;


                if (processAsMarkdown)
                {
                    if (!hasExtension)
                        path += ".md";

                    context.Items["MarkdownPath_OriginalPath"] = path;
                    context.Items["MarkdownPath_FolderConfiguration"] = folder;

                    // rewrite path to our controller so we can use _layout page
                    context.Request.Path = "/markdownprocessor/markdownpage";
                }
            }

            return _next(context);
        }
    }


    /// <summary>
    /// The Middleware Hookup extensions.
    /// </summary>
    public static class MarkdownPageProcessorMiddlewareExtensions
    {

        /// <summary>
        /// Configure the MarkdownPageProcessor in Startup.ConfigureServices.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMarkdownPageProcessor(this IServiceCollection services,
            Action<MarkdownPageProcessorConfiguration> configAction = null)
        {            
            var config = new MarkdownPageProcessorConfiguration();

            if (configAction != null)            
                configAction.Invoke(config);

            config.MarkdownProcessingFolders = 
                config.MarkdownProcessingFolders
                    .OrderBy(f => f.RelativePath)
                    .ToList();

            services.AddSingleton(config);
            
            return services;
        }


        /// <summary>
        /// Hook up the Markdown Page Processing functionality in the Startup.Configure method
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMarkdownPageProcessor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MarkdownProcessorMiddleware>();
        }
    }
}
