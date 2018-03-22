using Microsoft.AspNetCore.Html;

namespace Westwind.AspNetCore.Markdown
{
    public static class Markdown
    {
        #region Static Helpers 

        /// <summary>
        /// Renders raw markdown from string to HTML
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="usePragmaLines"></param>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        public static string Parse(string markdown, bool usePragmaLines = false, bool forceReload = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            var parser = MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown);
        }

        /// <summary>
        /// Renders raw Markdown from string to HTML.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="usePragmaLines"></param>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false)
        {
            return new HtmlString(Parse(markdown, usePragmaLines, forceReload));
        }

        #endregion
    }
}
