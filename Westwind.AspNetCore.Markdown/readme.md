# ASP.NET Core Markdown TagHelper and Markdown Parser using MarkDig

This small package provides Markdown support for your ASP.NET Core applications. It includes:

* **Markdown TagHelper** 
    *  Embed Markdown Content into Views and Pages
    *  Databind Markdown text
*  **Markdown Parser**
    *  Generate HTML to string
    *  Generate HtmlString for Razor usage
*  **Markdown Page Processor Middleware**
    *  Serve .md files as Markdown
    *  Serve extensionless files as Markdown
    *  Supports templated Markdown
*  [Uses the MarkDig Markdown Parser](https://github.com/lunet-io/markdig)


## Installation
You can install the package [from NuGet](https://www.nuget.org/packages/Westwind.AspNetCore.Markdown/) in Visual Studio or from the command line:


```ps
PM> install-package westwind.aspnetcore.markdown
```

or the `dotnet` command line:

```ps
dotnet add package westwind.aspnetcore.markdown
```

## Getting Started

* Install the NuGet Package (or add assemblies to projects)
* Register TagHelper in _ViewImports.cshtml
* Place a `<markdown>` TagHelper on the page
* Use `Markdown.ParseHtmlString()` in Razor Page expressions
* Rock on!

After installing the NuGet package **you have to register** the tag helper so MVC can find it. The easiest way to do this is to add it to the `_ViewImports.cshtml` file in your `Views\Shared` folder for MVC or the root for your `Pages` folder.

```
@addTagHelper *, Westwind.AspNetCore.Markdown
```


## Markdown TagHelper
The Markdown TagHelper allows you to embed static Markdown content into a `<markdown>` tag. The Tag supports both embedded content, or an attribute based value assignment or model binding via the `markdown` attribute.


### Literal Markdown Content
To use the literal content control you can simply place your Markdown text between the opening and closing `<markdown>` tags:

```html
<markdown>
    #### This is Markdown text inside of a Markdown block

    * Item 1
    * Item 2
 
    ### Dynamic Data is supported:
    The current Time is: @DateTime.Now.ToString("HH:mm:ss")

    ```cs
    // this c# is a code block
    for (int i = 0; i < lines.Length; i++)
    {
        line1 = lines[i];
        if (!string.IsNullOrEmpty(line1))
            break;
    }
    ```
</markdown>
```

The TagHelper turns the Markdown text into HTML, in place of the TagHelper content.

> ##### Razor Expression Evaluation
> Note that Razor expressions in the markdown content are supported and are expanded **before** the content is parsed by the TagHelper. This means you can embed dynamic values into the markdown content which gives you most of the flexibilty of Razor code now in Markdown. Additionally you can also generate additional Markdown as part of a Razor expression to make the Markdown even more dynamic.


> #### Styling is your Responsibility
> This parser only converts Markdown to HTML, it does nothing for styling or how that rendered HTML content is displayed. That's up to the host application's styling.
>
> Syntax highlighting can be accomplished by using a JavaScript code highlighting addin. You can check out the [Markdown.cshtml sample page](https://github.com/RickStrahl/Westwind.AspNetCore/blob/master/SampleWeb/Pages/Markdown.cshtml) to see how to use [HiLightJs ](https://highlightjs.org/).


### Markdown Attribute and DataBinding
In addition to the content you can also bind to the `markdown` attribute which allows for programmatic assignment and databinding.

```
@model MarkdownModel
@{
    Model.MarkdownText = "This is some **Markdown**!";
}

<markdown markdown="Model.MarkdownText" />
```

The `markdown` attribute accepts binding expressions you can bind Markdown for display from model values or other expressions easily.

### NormalizeWhiteSpace
Markdown is sensitive to leading spaces and given that you're likely to enter Markdown into the literal TagHelper in a code editor there's likely to be a leading block of white space. Markdown treats leading whitespace as significant - 4 spaces or a tab indicate a code block so if you have:

```html
<markdown>
    #### This is Markdown text inside of a Markdown block

    * Item 1
    * Item 2
 
    ### Dynamic Data is supported:
    The current Time is: @DateTime.Now.ToString("HH:mm:ss")

</markdown>
```

without special handling Markdown would interpret the entire markdown text as a single code block.

By default the TagHelper sets `normalize-whitespace="true"` which automatically strips common whitespace to all lines from the code block. Note that this is based on the formatting of the first non-blank line of code and works only if all code lines start with the same formatted whitespace.

Optionally you can also force justify your code and turn the setting to `false`:

```html
<markdown normalize-whitespace="false">
#### This is Markdown text inside of a Markdown block

* Item 1
* Item 2

### Dynamic Data is supported:
The current Time is: @DateTime.Now.ToString("HH:mm:ss")

</markdown>
```

This also works, but is hard to maintain in some code editors due to auto-code reformatting.


## Markdown Parsing
You can also use this component for simple Markdown parsing in code or your Razor pages.

### Markdown to String

```cs
string html = Markdown.Parse(markdownText)
```

### Markdown to Razor Html String

```cs
<div>@Markdown.ParseHtmlString(Model.ProductInfoMarkdown)</div>
```

## Markdown Page Processor Middleware
You can also set up your site to serve Markdown files from disk as self-contained Web pages. You can configure a folder hierarchy for serving `.md` files or extensionless urls that are mapped to underlying .md files and use a master template that renders the HTML.

To use this feature you need to do the following:

* Create a Markdown View Template (prefer: `~/Views/__MarkdownPageTemplate.cshtml`)
* Use `AddMarkdownPageProcessor()` to configure the page processing
* Use `UseMarkdownPageProcessor()` to hook up the middleware
* Create `.md` files for your content

### Create a Markdown Page View Template
The first thing that's required is a Markdown Page template that acts as a container for your markdown page on disk. The middleware reads in the Markdown file from disk, renders it to HTML and then uses the template to render the rendered Markdown into the template. 

The `ViewBag.RenderedMarkdown` is an `HtmlString` instance that contains the HTML text. At minimum you can create a page like this:

```html
@{
    Layout = "_Layout";
}
<div style="margin-top: 40px;">
    @ViewBag.RenderedMarkdown
</div>
```

This template can be a self contained file, or as I am doing here, it can explicitly reference a layout page so that the over layout matches the rest of your site.

Each individual folder hierarchy can have it's own template but all within that group have to use the same layout.

### Startup Configuration
As with any middleware components you need to configure the MarkdownPageProcessor middleware and hook it up for processing which is a two step process.

First you need to call `AddMarkdownPageProcessor()` to configure the Markdown processor. You need to specify the folders that the processor is supposed to work on.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMarkdownPageProcessor(config =>
    {
        var folderConfig = config.AddMarkdownProcessingFolder("/posts/");
        folderConfig.PreProcess = (folder, controller) => { controller.ViewBag.Model = "Custom Data here..."; };
    });
    
    services.AddMvc();
}
```

There are additional options including the ability to hook in a pre-processor that's fired on every controller hit. In the example I set a dummy value to the ViewBag that the template could potentially pick up and work with. For applications you might have a stock View model that provides access rights and other user logic that needs to fire to access the page and display the view. Using the `PreProcess` Action hook you can run just about any pre-processing logic and get values into the View if necessary via the `Controller.Viewbag`.

Additionally you also need to hook up the Middleware in the `Configure()` method.

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{  
    ...
    
    app.UseMarkdownPageProcessor();
    
    app.UseStaticFiles();
    app.UseMvc();
}
```        

The `UseMarkdownPageProcessor()` method hooks up the middleware into the pipeline.

### Create your Markdown Pages
Finally you need to create your Markdown pages in the folders you configured. Assume for a minute that you hooked up a `Posts` folder for Markdown Processing. The folder refers to the `\wwwroot\Posts` folder in your ASP.NET Core project. You can now create a new markdown file in that folder or any subfolder below it. I'm going to pretend I create blog post in a typical data folder structure. For example:

```
/Posts/2018/03/25/MarkdownTagHelper.md
```

I can now access this post using either:

http://localhost:59805/posts/2018/03/23/MarkdownTagHelper.md

or if extensionless URLs are configured:

http://localhost:59805/posts/2018/03/23/MarkdownTagHelper