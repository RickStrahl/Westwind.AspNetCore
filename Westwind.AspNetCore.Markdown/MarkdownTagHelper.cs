using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// An ASP.NET Server control that renders the embedded text as Markdown.
    /// </summary>

    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        /// <summary>
        /// Tries to strip whitespace before all lines based on the whitespace applied on the first line.")
        /// </summary>        
        [HtmlAttributeName("normalize-whitespace")]
        public bool NormalizeWhitespace { get; set; } = true;

        [HtmlAttributeName("markdown")]
        public ModelExpression Markdown { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            string text = null;
            if (Markdown != null)
                text = Markdown.Model?.ToString();

            if (text == null)
            {
                var content = await output.GetChildContentAsync();
                text = content.GetContent();
            }
            
            if (string.IsNullOrEmpty(text))
                return;

            text = text.Trim('\n', '\r');

            string markdown = NormalizeWhiteSpaceText(text);            

            var parser = MarkdownParserFactory.GetParser();
            var html = parser.Parse(markdown);

            output.TagName = null;  // Remove the <markdown> element
            output.Content.SetHtmlContent(html);
        }

        string NormalizeWhiteSpaceText(string text)
        {
            if (!NormalizeWhitespace || string.IsNullOrEmpty(text))
                return text;

            var lines = GetLines(text);
            if (lines.Length < 1)
                return text;

            string line1 = null;

            // find first non-empty line
            for (int i = 0; i < lines.Length; i++)
            {
                line1 = lines[i];
                if (!string.IsNullOrEmpty(line1))
                    break;
            }

            if (string.IsNullOrEmpty(line1))
                return text;

            string trimLine = line1.TrimStart();
            int whitespaceCount = line1.Length - trimLine.Length;
            if (whitespaceCount == 0)
                return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > whitespaceCount)
                    sb.AppendLine(lines[i].Substring(whitespaceCount));
                else
                    sb.AppendLine(lines[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        static string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null)
                return null;

            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }
    }
}
