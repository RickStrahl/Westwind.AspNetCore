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

            foreach (var folder in _configuration.MarkdownProcessingFolders)
            {
                bool hasMdExtension = path.EndsWith(".md");
                bool hasExtension = path.Contains(".");
                bool isRoot = path == "/";

                bool processAsMarkdown = false;

                

                // Root Url = everything is processed (not a good idea)
                if (isRoot)
                {
                    if (folder.RelativePath != "/")
                        continue;

                    if (folder.ProcessExtensionlessUrls && !hasExtension || hasMdExtension && folder.ProcessMdFiles)
                        processAsMarkdown = true;
                }
                else if(path.ToLower().StartsWith(folder.RelativePath.ToLower()) && (folder.ProcessExtensionlessUrls && !hasExtension || hasMdExtension && folder.ProcessMdFiles))
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

            if (context.Request.Path.Value.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
            {

                context.Items["MarkdownPath_OriginalPath"] = context.Request.Path.Value;
                // rewrite path to our controller so we can use _layout page
                context.Request.Path = "/markdown/markdownpage";
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
