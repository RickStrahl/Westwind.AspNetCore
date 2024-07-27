using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Westwind.Utilities;

namespace Westwind.Web
{

    /// <summary>
    /// The class that provides FormVariable to Model binding by matching
    /// FormVariable names to the model's properties including child
    /// properties. You can specify a
    ///
    /// The base behavior is similar to ASP.NET MVC's ModelBinder's
    /// binding operation minus the validation features.
    /// </summary>
    /// <remarks>
    /// Note on Child Property Separators:
    /// WebForms doesn't support '.' characters in ID= values so
    /// you will need to specify a FormVarPropertySeparator. A good
    /// value to use is '__' (two underscores) to allow for underscores
    /// in actual properties. If you know your properties don't have underscores
    /// then a single '_' will also work. The separator is converted to . when
    /// parsing for child property names.
    /// </remarks>
    public class FormVariableBinder
    {
        public HttpRequest Request { get; }

        /// <summary>
        /// An object to bind to
        /// </summary>
        public object Model { get; set; }

        ///// <summary>
        ///// Target should be an HttpRequest objectBinder
        ///// </summary>
        //public object Target {get; set; }

        /// <summary>
        /// The character used as a separator in the HTML form for child properties
        /// (ie. Address.Street or Address.Phone.Home)
        /// Default value is a .
        /// </summary>
        public string FormVarPropertySeparator { get; set; }

        /// <summary>
        /// An optional prefix on form variables to unbind.
        /// Can also specify multiple prefixes separated by commas.
        /// </summary>
        public string Prefixes { get; set; }

        /// <summary>
        /// List of exceptions that aren't to be bound. Uses the Form variable name.
        ///
        /// Values in the list can either be:
        /// * Full key name that includes the Prefix (ie. "Product.Price")
        /// * Partial key name that has stripped the Prefix (ie. "Price")
        /// </summary>
        public List<string> PropertyExclusionList { get; set; }

        /// <summary>
        /// Explicit List of properties that should be unbound.
        ///
        /// If you specify **any** values in this list **only** these
        /// keys will be checked for in the Request.Form collection - all
        /// others are excluded regardless of the exclusion list.
        ///
        /// Values in the list can either be:
        /// * Full key name that includes the Prefix (ie. "Product.Price")
        /// * Partial key name that has stripped the Prefix (ie. "Price")
        /// </summary>
        public List<string> PropertyInclusionList { get; set; }

        /// <summary>
        /// Binding Errors that occur on unbinding into the model
        /// </summary>
        public List<ValidationError> BindingErrors { get; set; } = new List<ValidationError>();


        /// <summary>
        /// Initialize the binder with property values
        /// </summary>
        /// <param name="request">HttpRequest object from which Form Variables are retrieved</param>
        /// <param name="model">The object to unbind to</param>
        /// <param name="propertyExclusions">Comma seperated list of properties to exclude</param>
        /// <param name="prefixes">Comma seperated list of prefixes that should be parsed.</param>
        /// <param name="propertyInclusionList">Comma separated list of explicit properties to include. Only these properties are used if specified.</param>
        public FormVariableBinder( HttpRequest request, object model, string propertyExclusions = null, string prefixes = null, string propertyInclusionList = null)
        {
            FormVarPropertySeparator = ".";
            Prefixes = prefixes ?? string.Empty;

            Model = model;
            Request = request;

            PropertyExclusionList = new List<string>();
            if (!string.IsNullOrEmpty(propertyExclusions))
                PropertyExclusionList.AddRange(propertyExclusions.Split(','));

            PropertyInclusionList = new List<string>();
            if (!string.IsNullOrEmpty(propertyInclusionList))
                PropertyInclusionList.AddRange(propertyInclusionList.Split(','));
        }

        /// <summary>
        /// Unbinds form variables into the specified target object
        /// </summary>
        /// <returns></returns>
        public bool Unbind()
        {
            this.BindingErrors.Clear();

            var props = Model.GetType().GetProperties(
                               BindingFlags.Public |
                               BindingFlags.Instance |
                               BindingFlags.Static);

            string[] prefixes =new string[0];

            if (!string.IsNullOrEmpty(Prefixes))
                prefixes = Prefixes.Split(new char[1] { ','},StringSplitOptions.RemoveEmptyEntries);
            var activePrefix = string.Empty;

            foreach (string reqKey in Request.Form.Keys)
            {
                // skip over 'system' form vars
                if (reqKey.StartsWith("__"))
                    continue;

                // if a prefix is specified but not provided skip over key
                if (!string.IsNullOrEmpty(Prefixes))
                {
                    if (prefixes.Length == 1)
                    {
                        if (!reqKey.StartsWith(Prefixes))
                            continue;

                        activePrefix = Prefixes;
                    }
                    else
                    {
                        // check each of the prefixes
                        foreach (string prefix in prefixes)
                        {
                            if (reqKey.StartsWith(prefix))
                            {
                                activePrefix = prefix;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(activePrefix))
                            continue;
                    }

                    string key = reqKey.Replace(this.FormVarPropertySeparator, ".");

                    if (!string.IsNullOrEmpty(activePrefix) )
                        key = key.Replace(activePrefix, "");

                    // Check for invalid property names,excluded keys or explicit inclusion props if provided
                    if (key.StartsWith(".") |
                        PropertyExclusionList.Any(s => s.ToLower() == reqKey.ToLower() || s.ToLower() == key.ToLower()) ||
                        (PropertyInclusionList.Count > 0 &&
                         !PropertyInclusionList.Any(s => s.ToLower() == reqKey.ToLower() || s.ToLower() == key.ToLower())) )
                    {
                        continue;
                    }

                    // We'll check the the first 'property' in chain against our property list
                    // to see whether it's a property we need to look up
                    string propertyKey = key;
                    if (key.IndexOf(".") > 1)
                        propertyKey = key.Substring(0, key.IndexOf("."));

                    PropertyInfo prop = props.Where(p => p.Name.ToLower() == propertyKey.ToLower()).FirstOrDefault();

                    // If we found a match  try to bind it - otherwise skip over
                    if (prop != null)
                    {
                        string val = Request.Form[reqKey].FirstOrDefault();
                        object objVal = null;

                        try
                        {
                            if (key == propertyKey)
                            {
                                objVal = ReflectionUtils.StringToTypedValue(val, prop.PropertyType);
                                prop.SetValue(Model, objVal, null);
                            }
                            else
                            {
                                // Retrieve the type and convert from string to type
                                prop = ReflectionUtils.GetPropertyInfoEx(Model, key);

                                // No match just continue looping thru form vars
                                if (prop == null)
                                    continue;

                                // Conversion
                                objVal = ReflectionUtils.StringToTypedValue(val, prop.PropertyType);

                                // Assign value to complex type
                                ReflectionUtils.SetPropertyEx(Model, key, objVal);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                                ex = ex.InnerException;

                            BindingErrors.Add(new ValidationError() {  Message = ex.Message, ID = key, ControlID = key });
                        }
                    }
                }
            }

            if (BindingErrors.Count > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Unbinds form variable data into a model object.
        /// </summary>
        /// <param name="model">Object to unbind to</param>
        /// <param name="propertyExceptions">Properties to skip</param>
        /// <param name="formvarPrefixes">Form Variable prefixes to include. Prefix is stripped. (txtName maps to Name)</param>
        /// <returns></returns>
        public static List<ValidationError> Unbind(HttpRequest Request,
                                                 object model,
                                                 string propertyExceptions = null,
                                                 string formvarPrefixes = null)
        {
            var binder = new FormVariableBinder(Request, model,  propertyExceptions, formvarPrefixes);
            binder.Prefixes = formvarPrefixes;

            binder.Unbind();

            return binder.BindingErrors;
        }
    }

}
