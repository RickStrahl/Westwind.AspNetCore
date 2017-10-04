using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Errors
{
    /// <summary>
    /// An error that is serialized to JSON. Includes an 
    /// isError property as interface marker along with a
    /// message and various error details that are displayed.
    /// </summary>
    public class ApiError
    {

        /// <summary>
        /// Error message or other message returned
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Error interface marker
        /// </summary>
        public bool isError { get; set; }

        /// <summary>
        /// Any additional error detail to display
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// Optional collection of errors.
        /// </summary>
        public ValidationErrorCollection errors { get; set; }


        /// <summary>
        /// Create a new API Error with a string message
        /// </summary>
        /// <param name="message"></param>
        public ApiError(string message)
        {
            this.message = message;
            isError = true;
        }


        /// <summary>
        /// Create a new API Error from model state dictionary 
        /// Error values.
        /// </summary>
        /// <param name="modelState"></param>
        public ApiError(ModelStateDictionary modelState)
        {
            isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                var errors = new ValidationErrorCollection();
                foreach(var modelStateItem in modelState)
                {
                    var key = modelStateItem.Key;
                    foreach(var val in modelStateItem.Value.Errors)
                    {
                        errors.Add(val.ErrorMessage, key, key);
                    }
                }

                message = errors.ToString();
            }            
        }
    }
}
