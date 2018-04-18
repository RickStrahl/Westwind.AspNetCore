using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.Tables;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMarkdown(config =>
            {
                // Simplest: Use all default settings
                config.AddMarkdownProcessingFolder("/docs/", "~/Pages/__MarkdownPageTemplate.cshtml");


                // Customized Configuration: Set FolderConfiguration options
                var folderConfig = config.AddMarkdownProcessingFolder("/posts/", "~/Pages/__MarkdownSimplestPageTemplate.cshtml");

                // Optional configuration settings
                folderConfig.ProcessExtensionlessUrls = true;  // default
                folderConfig.ProcessMdFiles = true; // default

                // Optional pre-processing
                folderConfig.PreProcess = (folder, controller) =>
                {
                    // controller.ViewBag.Model = new MyCustomModel();
                };

                // optional custom MarkdigPipeline (using MarkDig; for extension methods)
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UsePipeTables()
                        .UseGridTables()                        
                        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                        .UseAutoLinks() // URLs are parsed into anchors
                        .UseAbbreviations()
                        .UseYamlFrontMatter()
                        .UseEmojiAndSmiley(true)                        
                        .UseListExtras()
                        .UseFigures()
                        .UseTaskLists()
                        .UseCustomContainers()
                        .UseGenericAttributes();
                };
            });

            // We need to use MVC so we can use a Razor Configuration Template
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");            
            }

            app.UseMarkdown();
            
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path.Value.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
            //    {

            //        context.Items["MarkdownPath_OriginalPath"] = context.Request.Path.Value;
            //        // rewrite path to our controller so we can use _layout page
            //        context.Request.Path = "/markdown/markdownpage";
            //    }

            //    await next();
            //});


            app.UseStaticFiles();
            app.UseMvc();


        }
    }
}
