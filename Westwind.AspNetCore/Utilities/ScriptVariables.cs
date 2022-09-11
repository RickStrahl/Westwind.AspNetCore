#region License
/*
 **************************************************************
 *  Author: Rick Strahl
 *          © West Wind Technologies, 2008 2011
 *          http://www.west-wind.com/
 *
 * Created: 09/04/2018
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Westwind.Utilities;
using Westwind.Utilities.Properties;

namespace Westwind.AspNetCore.Utilities
{
    /// <summary>
    /// ScriptVariables allows you to easily push server side values into JavaScript
    /// code. It allows you add properties to a collection which can then be rendered
    /// into a JavaScript object using the ToString() or ToHtmlString(). Output is
    /// rendered as a single JavaScript object literal with properties for each
    /// item/value added. Any type of value or object can be added including nested
    /// objects and collections. It's an easy way to serialize complex data into
    /// a JavaScript code from the server.
    ///
    /// The component also supports posting back of updated values from the client
    /// and a generic Items[] collection that allows page data to update generic
    /// values and post them back to the server. The items are available
    ///
    /// This component produces either straight string or HtmlString output when used directly
    /// using the ToString() or HtmlString() methods for use in ASP.NET MVC or Web Pages,
    /// or can be used as WebForms control that automatically handles embedding of
    /// the script and deserialization of return values on the server.
    ///
    /// This component supports:&lt;&lt;ul&gt;&gt;
    /// &lt;&lt;li&gt;&gt; Creating individual client side variables
    /// &lt;&lt;li&gt;&gt; Dynamic values that are 'evaluated' in OnPreRender to
    /// pick up a value
    /// &lt;&lt;li&gt;&gt; Creating properties of ClientIDs for a given container
    /// &lt;&lt;li&gt;&gt; Changing the object values and POSTing them back on
    /// Postback
    /// &lt;&lt;/ul&gt;&gt;
    ///
    /// You create a script variables instance and add new keys to it:
    /// &lt;&lt;code lang="C#"&gt;&gt;
    /// ScriptVariables scriptVars = new ScriptVariables(this,"scriptVars");
    ///
    /// // Simple value
    /// scriptVars.Add("userToken", UserToken);
    ///
    /// AmazonBook tbook = new AmazonBook();
    /// tbook.Entered = DateTime.Now;
    ///
    /// // Complex value marshalled
    /// scriptVars.Add("emptyBook", tbook);
    ///
    /// scriptVars.AddDynamic("author", txtAuthor,"Text");
    ///
    /// // Cause all client ids to be rendered as scriptVars.formFieldId vars (Id
    /// postfix)
    /// scriptVars.AddClientIds(Form,true);
    /// &lt;&lt;/code&gt;&gt;
    ///
    /// In client code you can then access these variables:
    /// &lt;&lt;code lang="JavaScript"&gt;&gt;$( function() {
    /// 	alert(scriptVars.book.Author);
    /// 	alert(scriptVars.author);
    /// 	alert( $("#" + scriptVars.txtAmazonUrlId).val() );
    /// });&lt;&lt;/code&gt;&gt;
    /// </summary>
    public class ScriptVariables
    {

        /// <summary>Edit
        /// Internally holds all script variables declared
        /// </summary>
        Dictionary<string, object> ScriptVars = new Dictionary<string, object>();


        /// <summary>
        /// The name of the object generated in client script code
        /// </summary>
        public string ClientObjectName { get; set; } = "serverVars";

        public bool useCamelCase { get; set; }

        /// <summary>
        /// Internally tracked prefix code
        /// </summary>
        private StringBuilder sbPrefixScriptCode = new StringBuilder();

        /// <summary>
        /// Internally tracked postfix code
        /// </summary>
        private StringBuilder sbPostFixScriptCode = new StringBuilder();


        /// <summary>
        /// Constructor that optionally accepts the name of the
        /// variable that is to be created
        /// </summary>
        /// <param name="clientObjectName">Name of the JavaScript variable to create</param>
        public ScriptVariables(string clientObjectName = "serverVariables")
        {
        }

        public JsonSerializerSettings Setting { get; set; }

        /// <summary>
        /// Serializes value or object to JSON using:
        /// * Indented formatting for objects
        /// * Dates formatted as `new Data(3123312312)`
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value, bool useJavaScriptNaming = false)
        {
            var contractResolver = new DefaultContractResolver();

            if (useJavaScriptNaming)
                contractResolver.NamingStrategy = new CamelCaseNamingStrategy();

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver,

            };

            // serialize dates to new Date(xxxx)
            settings.Converters.Add(new JavaScriptDateTimeConverter());

            return JsonConvert.SerializeObject(value,settings);
        }


        /// <summary>
        /// Adds a property and value to the client side object to be rendered into
        /// JavaScript code. VariableName becomes a property on the object and the
        /// value will be properly converted into JavaScript Compatible JSON text.
        /// <seealso>Class ScriptVariables</seealso>
        /// </summary>
        /// <param name="variableName">
        /// The name of the property created on the client object.
        /// </param>
        /// <param name="value">
        /// The value that is to be assigned. Can be any simple type and most complex
        /// objects that can be serialized into JSON.
        /// </param>
        /// <example>
        /// &amp;lt;&amp;lt;code
        /// lang=&amp;quot;C#&amp;quot;&amp;gt;&amp;gt;ScriptVariables scriptVars = new
        ///  ScriptVariables(this,&amp;quot;serverVars&amp;quot;);
        ///
        /// // Add simple values
        /// scriptVars.Add(&amp;quot;name&amp;quot;,&amp;quot;Rick&amp;quot;);
        /// scriptVars.Add(&amp;quot;pageLoadTime&amp;quot;,DateTime.Now);
        ///
        /// // Add objects
        /// AmazonBook amazon = new AmazonBook();
        /// bookEntity book = amazon.New();
        ///
        /// scripVars.Add(&amp;quot;book&amp;quot;,book);
        /// &amp;lt;&amp;lt;/code&amp;gt;&amp;gt;
        /// </example>
        public void Add(string variableName, object value)
        {
            ScriptVars[variableName] = value;
        }

        /// <summary>
        /// Adds an entire dictionary of values
        /// </summary>
        /// <param name="values"></param>
        public void Add(IDictionary<string, object> values)
        {
            foreach (var item in values)
            {
                ScriptVars[item.Key] = item.Value;
            }
        }


        /// <summary>
        /// Any custom JavaScript code that is to immediately preceed the
        /// client object declaration. This allows setting up of namespaces
        /// if necesary for scoping.
        /// </summary>
        /// <param name="scriptCode"></param>
        public void AddScriptBefore(string scriptCode)
        {
            sbPrefixScriptCode.AppendLine(scriptCode);
        }

        /// <summary>
        /// Any custom JavaScript code that is to immediately follow the
        /// client object declaration. This allows setting up of namespaces
        /// if necesary for scoping.
        /// </summary>
        /// <param name="scriptCode"></param>
        public void AddScriptAfter(string scriptCode)
        {
            sbPostFixScriptCode.AppendLine(scriptCode);
        }

        ///// <summary>
        ///// Returns a value that has been updated on the client
        /////
        ///// Note this method will throw if it is not called
        ///// during PostBack or if AllowUpdates is false.
        ///// </summary>
        ///// <typeparam name="TType"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public TType GetValue<TType>(string key)
        //{
        //    HttpRequest Request = HttpContext.Current.Request;

        //    if (UpdateMode == AllowUpdateTypes.None || UpdateMode == AllowUpdateTypes.ItemsOnly)
        //        throw new InvalidOperationException("Can't get values if AllowUpdates is not set to true");

        //    if (Request.HttpMethod != "POST")
        //        throw new InvalidOperationException("GetValue can only be called during postback");

        //    // Get the postback value which is __ + ClientObjectName
        //    string textValue = PostBackValue;
        //    if (textValue == null)
        //        return default(TType);

        //    // Retrieve individual Url encoded value from the bufer
        //    textValue = WebUtils.GetUrlEncodedKey(textValue, key);
        //    if (textValue == null)
        //        return default(TType);

        //    // And deserialize as JSON
        //    object value = JsonSerializer.Deserialize(textValue, typeof(TType));

        //    return (TType)value;
        //}

        ///// <summary>
        ///// Returns a value from the client Items collection
        ///// </summary>
        ///// <typeparam name="TType"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public TType GetItemValue<TType>(string key)
        //{
        //    HttpRequest Request = HttpContext.Current.Request;

        //    if (UpdateMode == AllowUpdateTypes.None || UpdateMode == AllowUpdateTypes.PropertiesOnly)
        //        throw new InvalidOperationException(Resources.CanTRetrieveItemsInUpdateMode + UpdateMode);

        //    if (Request.HttpMethod != "POST")
        //        return default(TType); // throw new InvalidOperationException("GetValue can only be called during postback");

        //    // Get the postback value which is __ + ClientObjectName
        //    string textValue = PostBackValue;
        //    if (string.IsNullOrEmpty(textValue))
        //        return default(TType);

        //    // Retrieve individual Url encoded value from the buffer
        //    textValue = WebUtils.GetUrlEncodedKey(textValue, "_Items");
        //    if (string.IsNullOrEmpty(textValue))
        //        return default(TType);

        //    textValue = WebUtils.GetUrlEncodedKey(textValue, key);
        //    if (textValue == null)
        //        return default(TType);

        //    // And deserialize as JSON
        //    object value = JsonSerializer.Deserialize(textValue, typeof(TType));

        //    return (TType)value;
        //}


        /// <summary>
        /// Returns the rendered JavaScript as a string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Returns the rendered JavaScript for the generated object and name.
        /// Note this method returns only the generated object, not the
        /// related code to save updates.
        ///
        /// You can use this method with MVC Views to embedd generated JavaScript
        /// into the the View page.
        /// <param name="addScriptTags">If provided wraps the script text with script tags</param>
        /// </summary>
        public string ToString(bool addScriptTags)
        {
            if (ScriptVars.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            if (addScriptTags)
                sb.AppendLine("<script>");

            // Check for any prefix code and inject it
            if (sbPrefixScriptCode.Length > 0)
                sb.Append(sbPrefixScriptCode);

            // If the name includes a . assignment is made to an existing
            // object or namespaced reference - don't create var instance.
            if (!ClientObjectName.Contains("."))
                sb.Append("var ");

            sb.AppendLine(ClientObjectName + " = {");


            foreach (KeyValuePair<string, object> entry in ScriptVars)
            {
                if (entry.Key.StartsWith("."))
                {
                    // It's a dynamic key
                    string[] tokens = entry.Key.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string varName = tokens[0];
                    string property = tokens[1];


                    object propertyValue = null;
                    if (entry.Value != null)
                        propertyValue = ReflectionUtils.GetPropertyEx(entry.Value, property);

                    sb.AppendLine("\t" + varName + ": " + Serialize(propertyValue,useCamelCase) + ",");
                }
                else
                    sb.AppendLine("\t" + entry.Key + ": " + Serialize(entry.Value,useCamelCase) + ",");
            }

            // Strip off last comma plus CRLF
            if (sb.Length > 0)
                sb.Length -= 3;

            sb.AppendLine("\r\n};");

            if (sbPostFixScriptCode.Length > 0)
                sb.AppendLine(sbPostFixScriptCode.ToString());

            if (addScriptTags)
                sb.AppendLine("</script>\r\n");

            return sb.ToString();
        }


        /// <summary>
        /// Returns the script as an HTML string. Use this version
        /// with AsP.NET MVC to force raw unencoded output in Razor:
        ///
        /// @scriptVars.ToHtmlString()
        /// </summary>
        /// <param name="addScriptTags"></param>
        /// <returns></returns>
        public HtmlString ToHtmlString(bool addScriptTags = false)
        {
            return new HtmlString(ToString(addScriptTags));
        }

    }


    public enum AllowUpdateTypes
    {
        None,
        ItemsOnly,
        PropertiesOnly,
        All
    }
}
