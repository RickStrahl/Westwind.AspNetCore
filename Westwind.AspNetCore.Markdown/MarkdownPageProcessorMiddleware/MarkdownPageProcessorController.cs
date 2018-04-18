using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;
using Westwind.AspNetCore.Markdown.Utilities;

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
            var pageFile = HttpContext.Items["MarkdownPath_PageFile"] as string;
            if (!System.IO.File.Exists(pageFile))
                return NotFound();
            
            // string markdown = await File.ReadAllTextAsync(pageFile);
            string markdown;
            using (var fs = new FileStream(pageFile, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {                
                markdown = await sr.ReadToEndAsync();                
            }
            
            if (string.IsNullOrEmpty(markdown))
                return NotFound();

            var model = ParseMarkdownToModel(markdown);


                        
            ViewBag.RenderedMarkdown = Markdown.ParseHtmlString(markdown);
            
            if (folderConfig != null)
            {
                folderConfig.PreProcess?.Invoke(folderConfig, this);
                return View(folderConfig.ViewTemplate, model);
            }
            
            return View(MarkdownConfiguration.DefaultMarkdownViewTemplate, model);
        }

        private MarkdownModel ParseMarkdownToModel(string markdown, MarkdownProcessingFolder folderConfig = null)
        {
            var model = new MarkdownModel();

            if (folderConfig == null)
                folderConfig = new MarkdownProcessingFolder();

            if (folderConfig.ExtractTitle)
            {
                var firstLines = StringUtils.GetLines(markdown, 30);
                var firstLinesText = String.Join("\n", firstLines);

                // Assume YAML 
                if (markdown.StartsWith("---"))
                {
                    var yaml = StringUtils.ExtractString(firstLinesText, "---", "---", returnDelimiters: true);
                    if (yaml != null)
                        model.Title = StringUtils.ExtractString(yaml, "title: ", "\n");
                }

                if (model.Title == null)
                {
                    foreach (var line in firstLines.Take(10))
                    {
                        if (line.TrimStart().StartsWith("# "))
                        {
                            model.Title = line.TrimStart(new char[] {' ', '\t', '#'});
                            break;
                        }
                    }
                }
            }

            model.RawMarkdown = markdown;
            model.RenderedMarkdown = Markdown.ParseHtmlString(markdown);

            return model;
        }

    }
}
