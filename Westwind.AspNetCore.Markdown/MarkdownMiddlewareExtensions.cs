using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// The Middleware Hookup extensions.
    /// </summary>
    public static class MarkdownMiddlewareExtensions
    {

        /// <summary>
        /// Configure the MarkdownPageProcessor in Startup.ConfigureServices.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMarkdown(this IServiceCollection services,
            Action<MarkdownConfiguration> configAction = null)
        {            
            var config = new MarkdownConfiguration();

            if (configAction != null)            
                configAction.Invoke(config);

            MarkdownParserBase.HtmlTagBlackList = config.HtmlTagBlackList;

            if (config.ConfigureMarkdigPipeline != null)
                MarkdownParserMarkdig.ConfigurePipelineBuilder = config.ConfigureMarkdigPipeline;

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
        public static IApplicationBuilder UseMarkdown(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MarkdownPageProcessorMiddleware>();
        }
    }
}
