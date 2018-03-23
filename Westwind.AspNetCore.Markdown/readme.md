# ASP.NET Core Markdown TagHelper and Markdown Parser using MarkDig

This small package provides Markdown support for your ASP.NET Core applications. It includes:

* A Markdown TagHelper to embed Markdown Content into Pages
    *  Embed Markdown text content
    *  Databind Markdown
*  Markdown Parser
    *  Generate HTML to string
    *  Generate HtmlString for Razor usage
*  Uses the [MarkDig Markdown Parser](https://github.com/lunet-io/markdig)


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

## Usage
This package contains support for a Markdown TagHelper and a Markdown Parser that makes it easy to use Markdown content rendered as HTML in your applications.

### Markdown TagHelper
The Markdown TagHelper allows you to embed static Markdown content into a `<markdown>` tag. The Tag supports both embedded content, or an attribute based value assignment or model binding via the `markdown` attribute.


#### Literal Markdown Content
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


#### Markdown Attribute and DataBinding
In addition to the content you can also bind to the `markdown` attribute which allows for programmatic assignment and databinding.

```
@model MarkdownModel
@{
    Model.MarkdownText = "This is some **Markdown**!";
}

<markdown markdown="Model.MarkdownText" />
```

The `markdown` attribute accepts binding expressions you can bind Markdown for display from model values or other expressions easily.

#### NormalizeWhiteSpace
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


### Markdown Parsing
You can also use this component for simple Markdown parsing in code or your Razor pages.

#### Markdown to String

```cs
string html = Markdown.Parse(markdownText)
```

#### Markdown to Razor Html String

```cs
<div>@Markdown.ParseHtmlString(Model.ProductInfoMarkdown)</div>
```