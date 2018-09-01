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

        #region Html Sanitation
        /// <summary>
        /// Global list of tags that are cleaned up by the script sanitation
        /// as a pipe separated list.
        ///
        /// You can change this value which applies to the scriptScriptTags
        /// option, but it is an application wide global setting.
        /// 
        /// Default: script|iframe|object|embed|form
        /// </summary>




            
        internal static string HtmlTagBlackList { get; set; } = "script|iframe|object|embed|form";

protected static Regex _RegExScript = new Regex(
    $@"(<({HtmlTagBlackList})\b[^<]*(?:(?!<\/({HtmlTagBlackList}))<[^<]*)*<\/({HtmlTagBlackList})>)",
    RegexOptions.IgnoreCase | RegexOptions.Multiline);

protected static Regex _RegExJavaScriptHref = new Regex(
    @"<.*?(href|src|dynsrc|lowsrc)=.*?(javascript:).*?>.*?<\/a>",
    RegexOptions.IgnoreCase | RegexOptions.Multiline);

protected static Regex _RegExOnEventAttributes = new Regex(
    @"<.*?\s(on.{4,12}=([""].*?[""]|['].*?['])).*?(>|\/>)",
    RegexOptions.IgnoreCase | RegexOptions.Multiline);

/// <summary>
/// Parses out script tags that might not be encoded yet
/// </summary>
/// <param name="html"></param>
/// <returns></returns>
protected string ParseScript(string html)
{
    // Replace Script tags
    html = _RegExScript.Replace(html, string.Empty);

    // Remove javascript: directives
    var matches = _RegExJavaScriptHref.Matches(html);
    foreach (Match match in matches)
    {
        var txt = StringUtils.ReplaceString(match.Value, "javascript:", "unsupported:", true);
        html = html.Replace(match.Value, txt);
    }

    // Remove onxxxx event handlers from elements
    matches = _RegExOnEventAttributes.Matches(html);
    foreach (Match match in matches)
    {
        var txt = match.Value;
        if (match.Groups.Count > 1)
        {
            var onEvent = match.Groups[1].Value;
            txt = txt.Replace(onEvent, string.Empty);
            if (!string.IsNullOrEmpty(txt))
                html = html.Replace(match.Value, txt);
        }
    }

    return html;
}

        #endregion

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
