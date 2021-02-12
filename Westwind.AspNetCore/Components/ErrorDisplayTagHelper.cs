﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Components
{
    /// <summary>
    /// Taghelper to display a BootStrap Alert box and FontAwesome icon.
    /// 
    /// Message and Header values can be assigned from model values using
    /// standard Razor expressions.
    /// 
    /// The Helper only displays content when message or header are set
    /// otherwise the content is not displayed, so when binding to your 
    /// model and the model value is empty nothing displays.
    /// </summary>
    /// <remarks>
    /// Requires FontAwesome in addition to bootstrap in order to display icons    
    /// </remarks>
    [HtmlTargetElement("error-display")]
    public class ErrorDisplayTagHelper : TagHelper
    {
        /// <summary>
        /// the main message that gets displayed
        /// </summary>
        [HtmlAttributeName("message")]
        public string message { get; set; }

        /// <summary>
        /// Optional header that is displayed in big text. Use for 
        /// 'noisy' warnings and stop errors only please :-)
        /// The message is displayed below the header.
        /// </summary>
        [HtmlAttributeName("header")]
        public string header { get; set; }

        /// <summary>
        /// Font-awesome icon name without the fa- prefix.
        /// Example: info, warning, lightbulb-o, 
        /// If none is specified - "warning" is used
        /// To force no icon use "none"
        /// </summary>
        [HtmlAttributeName("icon")]
        public string icon { get; set; }

        /// <summary>
        /// CSS class. Handled here so we can capture the existing
        /// class value and append the BootStrap alert class.
        /// </summary>
        [HtmlAttributeName("class")]
        public string cssClass { get; set; }

        /// <summary>
        /// Optional - specifies the alert class used on the top level
        /// window. If not specified uses the same as the icon. 
        /// Override this if the icon and alert classes are different
        /// (often they are not).
        /// </summary>
        [HtmlAttributeName("alert-class")]
        public string alertClass { get; set; }

        /// <summary>
        /// If true embeds the message text as HTML. Use this 
        /// flag if you need to display HTML text. If false
        /// the text is HtmlEncoded.
        /// </summary>
        [HtmlAttributeName("message-as-raw-html")]
        public bool messageAsRawHtml { get; set; }


        /// <summary>
        /// If true embeds the header text as HTML. Use this 
        /// flag if you need to display raw HTML text. If false
        /// the text is HtmlEncoded, true the entire text left
        /// as raw HTML.
        /// </summary>
        [HtmlAttributeName("header-as-raw-html")]
        public bool headerAsRawHtml { get; set; }

        /// <summary>
        /// If true displays a close icon to close the alert.
        /// </summary>
        [HtmlAttributeName("dismissible")]
        public bool dismissible { get; set; }

        /// <summary>
        /// Error display instance - if provided overrides any other values set
        /// </summary>
        [HtmlAttributeName("error-display-model")]
        public ErrorDisplayModel   errorDisplay { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (errorDisplay != null)
            {
                message = errorDisplay.Message;
                header = errorDisplay.Header;
                icon = errorDisplay.Icon;
                messageAsRawHtml = errorDisplay.MessageAsRawHtml;
                headerAsRawHtml = errorDisplay.HeaderAsRawHtml;
                dismissible = errorDisplay.Dismissable;
                alertClass = errorDisplay.AlertClass;
            }

            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(header))
                return;

            if (string.IsNullOrEmpty(icon))
                icon = "warning";
            else
                icon = icon.Trim();
            if (icon == "none")
                icon = "";

            // assume alertclass to match icon  by default
            // override it when icon and alert class are diff (ie. info, info-circle)
            if (string.IsNullOrEmpty(alertClass))
                alertClass = icon;

            if (icon == "info")
                icon = "info-circle";
            if (icon == "warning")
                icon = "exclamation-triangle";
            if (icon == "danger")
            {
                icon = "exclamation-triangle";
                if (string.IsNullOrEmpty(alertClass))
                    alertClass = "alert-danger";
            }
            if (icon == "success")
            {
                icon = "check-circle";
                if (string.IsNullOrEmpty(alertClass))
                    alertClass = "success";
            }

            if (icon == "warning" || icon == "error" || icon == "danger")
                icon = icon + " text-danger"; // force to error color

            if (dismissible && !alertClass.Contains("alert-dismissible"))
                alertClass += " alert-dismissible";

            string messageText = !messageAsRawHtml ? System.Net.WebUtility.HtmlEncode(message) : message;
            string headerText = !headerAsRawHtml ? System.Net.WebUtility.HtmlEncode(header) : header;

            output.TagName = "div";

            // fix up CSS class            
            if (cssClass != null)
                cssClass = cssClass + " alert alert-" + alertClass;
            else
                cssClass = "alert alert-" + alertClass;
            output.Attributes.Add("class", cssClass);
            output.Attributes.Add("role", "alert");

            StringBuilder sb = new StringBuilder();

            if (dismissible)
                sb.Append(
                    "<button type =\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n" +
                    "   <span aria-hidden=\"true\">&times;</span>\r\n" +
                    "</button>\r\n");

            if (string.IsNullOrEmpty(header))
                sb.AppendLine($"<i class='fa fa-{icon}'></i> {messageText}");
            else
            {
                sb.Append(
                    $"<h3><i class='fas fa-{icon}'></i> {headerText}</h3>\r\n" +
                    "<hr/>\r\n" +
                    $"{messageText}\r\n");
            }

            if (errorDisplay != null)
            {
                if (errorDisplay.DisplayErrors.Count > 0)
                {
                    sb.AppendLine("<hr/>");
                    sb.AppendLine(errorDisplay.DisplayErrors.ToHtml());
                }
            }

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
