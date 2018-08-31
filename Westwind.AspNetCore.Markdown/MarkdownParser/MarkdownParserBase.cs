using System.Linq;
using System.Text.RegularExpressions;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Base class that includes various fix up methods for custom parsing
    /// that can be called by the specific implementations.
    /// </summary>
    public abstract class MarkdownParserBase : IMarkdownParser
    {
        protected static Regex strikeOutRegex = 
            new Regex("~~.*?~~", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Parses markdown
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="stripScript">Strips script tags and javascript: text</param>
        /// <returns></returns>
        public abstract string Parse(string markdown, bool stripScript = false);
        
        /// <summary>
        /// Parses strikeout text ~~text~~. Single line (to linebreak) allowed only.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseStrikeout(string html)
        {
            if (html == null)
                return null;

            var matches = strikeOutRegex.Matches(html);
            foreach (Match match in matches)
            {
                string val = match.Value;

                if (match.Value.Contains('\n'))
                    continue;

                val = "<del>" + val.Substring(2, val.Length - 4) + "</del>";
                html = html.Replace(match.Value, val);
            }

            return html;
        }

        static readonly Regex YamlExtractionRegex = new Regex("^---[\n,\r\n].*?^---[\n,\r\n]", RegexOptions.Singleline | RegexOptions.Multiline);

        /// <summary>
        /// Strips 
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public string StripFrontMatter(string markdown)
        {
            string extractedYaml = null;
            var match = YamlExtractionRegex.Match(markdown);
            if (match.Success)
                extractedYaml = match.Value;

            if (!string.IsNullOrEmpty(extractedYaml))
                markdown = markdown.Replace(extractedYaml, "");

            return markdown;
        }

        protected static Regex _RegExScript = new Regex(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",RegexOptions.IgnoreCase);
        protected static Regex _RegExJavaScriptHref = new Regex(@"<a.*?href=.*?(javascript:).*?>.*?<\/a>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Parses out script tags that might not be encoded yet
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseScript(string html)
        {
            html = _RegExScript.Replace(html, string.Empty);
            var matches = _RegExJavaScriptHref.Matches(html);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var txt = StringUtils.ReplaceString(match.Value, "javascript:", "unsupported:", true);
                    html = html.Replace(match.Value, txt);
                }
            }

            return html;
        }

        public static Regex fontAwesomeIconRegEx = new Regex(@"@icon-.*?[\s|\.|\,|\<]");

        /// <summary>
        /// Post processing routine that post-processes the HTML and 
        /// replaces @icon- with fontawesome icons
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseFontAwesomeIcons(string html)
        {
            var matches = fontAwesomeIconRegEx.Matches(html);
            foreach (Match match in matches)
            {
                string iconblock = match.Value.Substring(0, match.Value.Length - 1);
                string icon = iconblock.Replace("@icon-", "");
                html = html.Replace(iconblock, "<i class=\"fa fa-" + icon + "\"></i> ");
            }

            return html;
        }
        
    }
}
