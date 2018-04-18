using Microsoft.AspNetCore.Html;

namespace Westwind.AspNetCore.Markdown
{
    public class MarkdownModel
    {
        public string Title { get; set; }
        public HtmlString RenderedMarkdown { get; set; }
        public string RawMarkdown { get; set; }
        public string YamlHeader { get; set; }
    }
}