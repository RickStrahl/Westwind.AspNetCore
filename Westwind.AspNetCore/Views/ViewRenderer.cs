using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Westwind.AspNetCore.Properties;

namespace Westwind.AspNetCore.Views
{

    /// <summary>
    /// Allows you to render a view to string.
    /// </summary>
    public class ViewRenderer
    {
        /// <summary>
        /// Renders a view to string.
        /// </summary>
        /// <param name="viewName">Name of the view to render. Use Controller relative view name syntax</param>
        /// <param name="model">Model data to pass in</param>
        /// <param name="controllerContext">A controller context needed to process this View</param>
        /// <param name="isPartial">Renders either a partial or main page (default)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<string> RenderViewToStringAsync(
            string viewName, object model,
            ControllerContext controllerContext,
            bool isPartial = false)
        {
            var actionContext = controllerContext as ActionContext;
            var serviceProvider = controllerContext.HttpContext.RequestServices;

            var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            if (razorViewEngine == null)
                throw new InvalidOperationException( Resources.RazorViewEngineNotAvailable);

            var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, !isPartial);

                if (viewResult?.View == null)
                {
                    throw new ArgumentException(string.Format(Resources.ViewNameDoesNotMatchAvailableView,viewName));
                }

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    { Model = model };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }



        }

        /// <summary>
        /// Creates a controller context from a base Url and without an Http Context
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static ControllerContext CreateControllerContext(string baseUrl)
        {
            var httpContext = new DefaultHttpContext();

            var currentUri = new Uri(baseUrl); // Here you have to set your url if you want to use links in your email
            httpContext.Request.Scheme = currentUri.Scheme;
            httpContext.Request.Host = HostString.FromUriComponent(currentUri);

            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            return  new ControllerContext(actionContext);
        }
    }

}
