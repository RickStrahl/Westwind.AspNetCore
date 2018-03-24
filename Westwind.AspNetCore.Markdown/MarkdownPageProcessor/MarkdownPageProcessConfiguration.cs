using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Westwind.AspNetCore.Markdown
{
    public class MarkdownPageProcessorConfiguration
    {
        /// <summary>
        /// List of relative virtual folders where any extensionless URL is 
        /// matched to an .md file on disk
        /// </summary>
        public List<MarkdownProcessingFolder> MarkdownProcessingFolders { get; set; } = new List<MarkdownProcessingFolder>();


        /// <summary>
        /// Adds a folder to the list of folders that are to be 
        /// processed by this middleware. 
        /// </summary>
        /// <param name="path">The path to work on. Examples: /docs/ or /classes/docs/.</param>
        /// <param name="viewTemplate">Path to a View Template. Defaults to: ~/Views/__MarkdownPageTemplate.cshtml</param>
        /// <param name="processMdFiles">Process files with an .md extension</param>
        /// <param name="processExtensionlessUrls">Process extensionless Urls as Markdown. Assume matching .md file is available that holds the actual Markdown text</param>
        /// <returns></returns>
        public MarkdownProcessingFolder AddMarkdownProcessingFolder(string path, string viewTemplate = null,
            bool processMdFiles = true, bool processExtensionlessUrls = true)
        {

            if (!path.StartsWith("/"))
                path = "/" + path;
            if (!path.EndsWith("/"))
                path = "/" + path + "/";

            var folder = new MarkdownProcessingFolder()
            {
                RelativePath = path,
                ProcessMdFiles = processMdFiles,
                ProcessExtensionlessUrls = processExtensionlessUrls
            };

            if (!string.IsNullOrEmpty(viewTemplate))
                folder.ViewTemplate = viewTemplate;

            MarkdownProcessingFolders.Add(folder);

            return folder;
        }
    }

    public class MarkdownProcessingFolder
    {

        /// <summary>
        /// Relative path
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// View Template to use to render the Markdown page
        /// </summary>
        public string ViewTemplate { get; set; } = "~/Views/__MarkdownPageTemplate.cshtml";


        /// <summary>
        /// If true processes files with .md extension
        /// </summary>
        public bool ProcessMdFiles { get; set; } = true;

        /// <summary>
        /// If true processes extensionless Urls in the the folder hierarchy as Markdown
        /// and expects a matching .md file
        /// </summary>
        public bool ProcessExtensionlessUrls { get; set; } = true;


        /// <summary>
        /// Function that can be set to be called before the Markdown View is fired.
        /// Use this method to potentially add additional data into the ViewBag you 
        /// might want access to in the 
        /// 
        /// </summary>
        public Action<MarkdownProcessingFolder, Controller> PreProcess
        { get; set; }
    }
}