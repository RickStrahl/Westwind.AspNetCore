using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;

namespace SampleWeb.Controllers
{

    /// <summary>
    /// A generic controll implementation
    /// </summary>
    public class MarkdownPageProcessorController : Controller
    {
        public MarkdownConfiguration MarkdownProcessorConfig { get; }
        private readonly IHostingEnvironment hostingEnvironment;

        public MarkdownPageProcessorController(IHostingEnvironment hostingEnvironment,
            MarkdownConfiguration config)
        {
            MarkdownProcessorConfig = config;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("markdownprocessor/markdownpage")]
        public async Task<IActionResult> MarkdownPage()
        {            
            var basePath = hostingEnvironment.WebRootPath;
            var relativePath = HttpContext.Items["MarkdownPath_OriginalPath"] as string;
            if (relativePath == null)
                return NotFound();

            var folderConfig = HttpContext.Items["MarkdownPath_FolderConfiguration"] as MarkdownProcessingFolder;
            
            relativePath = NormalizePath(relativePath).Substring(1);
            var pageFile = Path.Combine(basePath,relativePath);            
            if (!System.IO.File.Exists(pageFile))
                return NotFound();

            var markdown = await System.IO.File.ReadAllTextAsync(pageFile);
            if (string.IsNullOrEmpty(markdown))
                return NotFound();

            ViewBag.RenderedMarkdown = Markdown.ParseHtmlString(markdown);

            if (folderConfig != null)
            {
                folderConfig.PreProcess?.Invoke(folderConfig, this);
                return View(folderConfig.ViewTemplate);
            }

            return View(MarkdownConfiguration.DefaultMarkdownViewTemplate);
        }

        /// <summary>
        /// Normalizes a file path to the operating system default
        /// slashes.
        /// </summary>
        /// <param name="path"></param>
        static string NormalizePath(string path)
        {
            //return Path.GetFullPath(path); // this always turns into a full OS path

            if (string.IsNullOrEmpty(path))
                return path;

            char slash = Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }
    }
}
