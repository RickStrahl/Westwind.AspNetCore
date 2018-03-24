using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            services.AddRouting();

            services.AddMarkdownPageProcessor(config =>
            {
                var folderConfig = config.AddMarkdownProcessingFolder("/docs/");
                folderConfig.PreProcess = (folder, controller) => { controller.ViewBag.Model = "Custom Data here..."; };
            });
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

            app.UseMarkdownPageProcessor();
            
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
