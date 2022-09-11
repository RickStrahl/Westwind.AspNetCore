using System.Web;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Westwind.AspNetCore.Extensions;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Components
{
    /// <summary>
    /// An error display component that allows rich rendering of individual messages
    /// as well as validation error messages.
    ///
    /// Object should be passed in to view end rendered with
    /// &lt;%= ((ErrorDisplay) ViewData["ErrorDisplay"]).Show(450,true) %&gt;
    /// or via a view.
    ///
    /// Relies on several CSS Styles:
    /// .errordisplay, errordisplay-text, errordisplay-warning-icon, errordisplay-info-icon
    /// The icon links link to images.
    /// </summary>
    public class ErrorDisplayModel
    {
        /// <summary>
        /// The message that is displayed
        /// </summary>
        public string Message { get; set; }

        public string Header { get; set; }


        /// <summary>
        /// Name of an font-awesome icon to display
        /// 'warning', 'error', 'info', 'success' have special
        /// meaning and display custom colors
        /// </summary>
        public string Icon { get; set; } = "warning";

        public string AlertClass { get; set; } = "warning";

        /// <summary>
        /// Flag that determines whether the message is displayed
        /// as HTML or as text. By default message is encoded as text (false).
        /// </summary>
        public bool MessageAsRawHtml { get; set; } = false;

        /// <summary>
        /// Flag that determines whether the message is displayed
        /// as HTML or as text. By default message is encoded as text (false).
        /// </summary>
        public bool HeaderAsRawHtml { get; set; } = false;

        /// <summary>
        /// Determines whether the alert can be closed
        /// </summary>
        public bool Dismissable { get; set; } = false;

        /// <summary>
        /// Determines whether there is a message present.
        /// </summary>
        public bool HasMessage
        {
            get
            {
                if (DisplayErrors.Count > 0 && string.IsNullOrEmpty(Message))
                    Message = "Please correct the following errors:";

                return !string.IsNullOrEmpty(Message);
            }
        }

        /// <summary>
        /// Timeout in milliseconds before the error display is hidden
        /// </summary>
        public int Timeout { get; set; }


        /// <summary>
        /// Holds a modelstate errors collection
        /// </summary>
        public ValidationErrorCollection DisplayErrors { get; set; } = new ValidationErrorCollection();


        public void ShowError(string errorMessage, string header = null)
        {
            Icon = "warning";
            Message = errorMessage;
            if (!string.IsNullOrEmpty(header))
                Header = header;

        }

        public void ShowWarning(string errorMessage, string header = null)
        {
            Icon = "warning";
            AlertClass = "warning";
            Message = errorMessage;
            if (!string.IsNullOrEmpty(header))
                Header = header;

        }

        public void ShowInfo(string message,string header = null)
        {
            Icon = "info";
            AlertClass = "info";
            Message = message;
            if (!string.IsNullOrEmpty(header))
                Header = header;

        }

        public void ShowSuccess(string message, string header = null)
        {
            Icon = "success";
            AlertClass = "success";
            Message = message;
            if (!string.IsNullOrEmpty(header))
                Header = header;
        }


        /// <summary>
        /// Adds ModelState errors to the validationErrors
        /// </summary>
        /// <param name="modelErrors"></param>
        public void AddMessages(ModelStateDictionary modelErrors, string fieldPrefix = null)
        {
            fieldPrefix = fieldPrefix ?? string.Empty;

            foreach(var state in modelErrors)
            {
                if ((state.Value.Errors.Count > 0))
                    DisplayErrors.Add(state.Value.Errors[0].ErrorMessage,fieldPrefix + state.Key);
            }
        }

        /// <summary>
        /// Adds an existing set of Validation Errors to the DisplayErrors
        /// </summary>
        /// <param name="validationErrors"></param>
        public void AddMessages(ValidationErrorCollection validationErrors,string fieldPrefix = null)
        {
            fieldPrefix = fieldPrefix ?? string.Empty;

            foreach (ValidationError error in validationErrors)
            {
                DisplayErrors.Add(error.Message,fieldPrefix + error.ControlID);
            }
        }

        /// <summary>
        /// Tries to populate the error display object from the request
        /// object's Query and Form Variables.
        /// Variables:
        /// error-message, error-icon, error-class
        /// </summary>
        /// <param name="request"></param>
        public void FromRequest(HttpRequest request)
        {
            Message = request.Params("error-message");
            var icon = request.Params("error-icon");
            if (!string.IsNullOrEmpty(icon))
            {
                Icon = icon;
                AlertClass = icon;
            }
            var alertClass = request.Params("error-class");
            if (!string.IsNullOrEmpty(alertClass))
                AlertClass = alertClass;
        }


        /// <summary>
        /// Adds an individual model error
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="control"></param>
        public void AddMessage(string errorMessage, string control = null)
        {
            DisplayErrors.Add(errorMessage,control);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Message))
                return Message;

            if (DisplayErrors.Count > 0)
                return DisplayErrors.ToString();

            return string.Empty;
        }
    }

}
