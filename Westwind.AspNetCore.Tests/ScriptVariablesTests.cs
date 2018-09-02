using System;
using Microsoft.AspNetCore.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Westwind.AspNetCore.Utilities;

namespace Westwind.AspNetCore.Tests
{
    [TestClass]
    public class ScriptVariablesTests
    {
        [TestMethod]
        public void BasicScriptVariablesWithScriptTags()
        {
            var scriptVars = new ScriptVariables();
            scriptVars.Add("name", "Rick");
            scriptVars.Add("timestamp", DateTime.UtcNow);

            var code = scriptVars.ToString();

            Console.WriteLine(code);
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("new Date("));
        }

        [TestMethod]
        public void BasicScriptVariablesWithScriptTagsToHtmlString()
        {
            var scriptVars = new ScriptVariables();
            scriptVars.Add("name", "Rick");
            scriptVars.Add("timestamp", DateTime.UtcNow);

            var code = scriptVars.ToHtmlString();

            Console.WriteLine(code);
            Assert.IsNotNull(code);
            Assert.IsTrue(code.GetType() ==  typeof(HtmlString));
            Assert.IsTrue(code.ToString().Contains("new Date("));
        }


        [TestMethod]
        public void RawValueSerialization()
        {

            var value = new
            {
                Name="Rick",
                Entered = DateTime.UtcNow,
                Accessed = 10
            };
            
            var result = ScriptVariables.Serialize(value);

            Console.WriteLine(result);
        }

    }
}
