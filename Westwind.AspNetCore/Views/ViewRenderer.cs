using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<string> RenderViewToStringAsync(
            string viewName, object model,
            ControllerContext controllerContext,
            bool isPartial = false)
        {
            var actionContext = controllerContext as ActionContext;
            var serviceProvider = controllerContext.HttpContext.RequestServices;

            var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, !isPartial);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
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
    }

}
