using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Utilities
{
    public static class WebUtils
    {
        #region JSON Value Encoding

        static DateTime DAT_JAVASCRIPT_BASEDATE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string that is returned in double quotes.
        ///
        /// The string returned includes outer quotes:
        /// "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <returns>JSON encoded string</returns>
        public static string EncodeJsString(string text)
        {
            if (text == null)
                return "null";

            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || c == '<' || c == '>')
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");


            return sb.ToString();
        }

        /// <summary>
        /// Parses a JSON string into a string value
        /// </summary>
        /// <param name="encodedString">JSON string</param>
        /// <returns>unencoded string</returns>
        public static string DecodeJsString(string encodedString)
        {
            // actual value of null is not valid for
            if (encodedString == null)
                return null;

            // null as a string is a valid value for a string
            if (encodedString == "null")
                return null;

            // Has to be bracketed in quotes
            if (!encodedString.StartsWith("\"") || !encodedString.EndsWith("\""))
                encodedString = "\"" + encodedString + "\"";

            if (encodedString == "\"\"")
                return string.Empty;

            // strip off leading and trailing quote chars
            encodedString = encodedString.Substring(1, encodedString.Length - 2);

            // Escape the double escape characters in json ('real' backslash)  temporarily to alternate chars
            const string ESCAPE_ESCAPECHARS = @"^#^#";

            encodedString = encodedString.Replace(@"\\", ESCAPE_ESCAPECHARS);

            encodedString = encodedString.Replace(@"\r", "\r");
            encodedString = encodedString.Replace(@"\n", "\n");
            encodedString = encodedString.Replace(@"\""", "\"");
            encodedString = encodedString.Replace(@"\t", "\t");
            encodedString = encodedString.Replace(@"\b", "\b");
            encodedString = encodedString.Replace(@"\f", "\f");

            if (encodedString.Contains("\\u"))
                encodedString = Regex.Replace(encodedString, @"\\u....",
                                      new MatchEvaluator(UnicodeEscapeMatchEvaluator));

            // Convert escaped characters back to the actual backslash char
            encodedString = encodedString.Replace(ESCAPE_ESCAPECHARS, "\\");

            return encodedString;
        }


        /// <summary>
        /// Converts a .NET date to a JavaScript JSON date value.
        /// </summary>
        /// <param name="date">.Net Date</param>
        /// <returns></returns>
        public static string EncodeJsDate(DateTime date, JsonDateEncodingModes dateMode = JsonDateEncodingModes.ISO)
        {
            TimeSpan tspan = date.ToUniversalTime().Subtract(DAT_JAVASCRIPT_BASEDATE);
            double milliseconds = Math.Floor(tspan.TotalMilliseconds);

            // ISO 8601 mode string "2009-03-28T21:55:21.1234567Z"
            if (dateMode == JsonDateEncodingModes.ISO)
                // this is the same format that browser JSON formatters produce
                return string.Concat("\"", date.ToString("yyyy-MM-ddThh:mm:ss.fffZ"), "\"");

            // raw date expression - new Date(1227578400000)
            if (dateMode == JsonDateEncodingModes.NewDateExpression)
                return "new Date(" + milliseconds + ")";

            // MS Ajax style string: "\/Date(1227578400000)\/"
            if (dateMode == JsonDateEncodingModes.MsAjax)
            {
                StringBuilder sb = new StringBuilder(40);
                sb.Append(@"""\/Date(");
                sb.Append(milliseconds);


                // Add Timezone
                sb.Append((TimeZoneInfo.Local.GetUtcOffset(date).Hours * 100).ToString("0000").PadLeft(4, '0'));

                sb.Append(@")\/""");
                return sb.ToString();
            }

            throw new ArgumentException("Date Format not supported.");
        }

        /// <summary>
        /// Matchevaluated to unescape string encoded Unicode character in the format of \u03AF
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private static string UnicodeEscapeMatchEvaluator(Match match)
        {
            // last 4 digits are hex value
            string hex = match.Value.Substring(2);
            char val = (char)ushort.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return val.ToString();
        }
        #endregion


        /// <summary>
        /// Resolves of a virtual Url to a fully qualified Url. This version
        /// requires that you provide a basePath, and if returning an absolute
        /// Url a host name.
        ///
        /// * ~/ ~ as base path
        /// * / as base path
        /// * https:// http:// return as is
        /// * Any relative path: returned as is
        /// * Empty or null: returned as is
        /// </summary>
        /// <remarks>Requires that you have access to an active Request</remarks>
        /// <param name="context">The HttpContext to work with (extension property)</param>
        /// <param name="url">Url to resolve</param>
        /// <param name="basepath">
        /// Optionally provide the base path to normalize for.
        /// Format: `/` or `/docs/`
        /// </param>
        /// <params name="currentpath">
        /// If you want to resolve relative paths you need to provide
        /// the current request path (should be a path not a page!)
        /// </params>
        /// <param name="returnAbsoluteUrl">If true returns an absolute Url( ie. `http://` or `https://`)</param>
        /// <param name="ignoreRelativePaths">
        /// If true doesn't resolve any relative paths by not prepending the base path.
        /// If false are returned as is.
        /// </param>
        /// <param name="ignoreRootPaths">
        /// If true doesn't resolve any root (ie. `/` based paths) by not prepending the base path.
        /// If false are returned as is
        /// </param>
        /// <returns>Updated path</returns>
        public static string ResolveUrl(
            string url,
            string basepath = "/",
            string currentPathForRelativeLinks = null,
            bool returnAbsoluteUrl = false,           
            bool ignoreRootPaths = false,
            string absoluteHostName = null,
            string absoluteScheme = "https://")
        {
            if (string.IsNullOrEmpty(url) ||
                url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return url;

            // final base path format will be: / or /docs/
            if (string.IsNullOrEmpty(basepath))
                basepath = "/";


            if (string.IsNullOrEmpty(basepath) || basepath == "/")
                basepath = "/";
            else
                basepath = "/" + basepath.Trim('/') + "/"; // normalize

            if (returnAbsoluteUrl)
            {
                if (string.IsNullOrEmpty(absoluteHostName))
                    throw new ArgumentException("Host name is required if you return absolute Urls");

                basepath = $"{absoluteScheme}://{absoluteHostName}/{basepath.TrimStart('/')}";
            }

            if (url.StartsWith("~/"))
                url = basepath + url.Substring(2);
            else if (url.StartsWith("~"))
                url = basepath + url.Substring(1);
            // translate root paths
            else if (url.StartsWith("/"))
            {
                if (!ignoreRootPaths && !url.StartsWith(basepath, StringComparison.OrdinalIgnoreCase))
                {
                    url = basepath + url.Substring(1);
                }
                // else pass through as is
            }
            else if (!string.IsNullOrEmpty(currentPathForRelativeLinks))
            {
                url = basepath + currentPathForRelativeLinks.Trim('/') + "/" + url.TrimStart('/');
            }

            // any relative Urls we can't do anything with
            // so return them as is and hope for the best

            return url;
        }
    }


    /// <summary>
    /// Enumeration that determines how JavaScript dates are
    /// generated in JSON output
    /// </summary>
    public enum JsonDateEncodingModes
    {
        NewDateExpression,
        MsAjax,
        ISO
    }
}
