# Westwind.AspNetCore Change Log
<small>[Nuget](https://www.nuget.org/packages/Westwind.AspNetCore/) &bull; [Github](https://github.com/RickStrahl/Westwind.AspNetCore)</small>


### Version 3.5
* **Add explicit support for .NET 6.0**  
Added support for .NET 6.0 and reset targeting for `net60;net50;netcoreapp3.1`.

* **ViewRenderer for Razor Views**  
Allows rendering of Razor views to string, based on an active ControllerContext.

* **Custom Headers Middleware**  
Added custom HTTP Headers middleware that allows adding and removing of HTTP headers to every request.

### Version 3.4

* **Add explicit support for .NET 5.0**
Added `net5.0` target and removed the `netcoreapp2.1` target.

* **IQueryCollectionExtensions**  
Added helpers for retrieving values from a StringCollection used in Request collections like Query and Form.

### Version 3.3

* **HttpRequest.IsLocal() Extension Method**  
Lets you quickly determine if a request is running on the local IP Address.

* **Add new BaseApiController Class**  
Add new BaseApiController class that by default adds the `ApiExceptionFilterAttribute` and the new `UserStateBaseApiControllerFilterAttribute` properties for transforming exceptions and automatically parsing Identity claims into a UserState object if provided.

* **New `UserStateBaseApiControllerFilterAttribute`**  
Filter that can be used to automatically parse UserState objects if they are provided in the Identity claims. Set UserState in Authentication methods when creating tokens or cookies and embed a `UserState` claim with `new Claim("UserState", UserState.ToString())` to add persisted token/cookie state that is returned when requests are authenticated.

* **New `HostEnvironmentAbstraction` and `LegacyHostEnvironment` Classes**  
These two classes provide two separate mechanisms for making hosting environments work across .NET Core 2.x and 3.x. `HostEnvironmentAbstraction` provides a wrapper that exposes a host environment for the appropriate environment consistently, while `LegacyHostEnvironment` provides an `IWebHostEnvironment` implementation for .NET Core 2.x.

### Version 3.2

* **Removed `Westwind.AspNetCore.Markdown**  
Removed the Markdown features and moved them into a separate, self-contained project.

### Version 3.0.50

* **Markdown TagHelper Filename Property**  
You can now include Markdown content from the site's file system using the `<markdown>` TagHelper. This makes it easy to break out large text blocks and edit and maintain them easily in your favorite Markdown editor vs. editing inline the HTML content.

### Version 3.0.36

* **Add better XSS Support for Markdown Parser**  
There's now optional, configurable handling for removing `<script>`,`<iframe>`,`<form>` etc. tags, `javascript:` directives, and `onXXXX` event handlers on raw HTML elements in the document. For more info see blog post: [Markdown and Cross Site Scripting](https://weblog.west-wind.com/posts/2018/Aug/31/Markdown-and-Cross-Site-Scripting).

* **WebUtils.JsonString(), WebUtils.JsonDate()**  
Added JSON helpers that facilitate encoding and decoding strings and dates to JSON without having to force use of a full JSON parser. Useful for emedding JSON values into script content.

### Version 3.0.25

* **Add Markdown Page Handler**  
Added a new page handler that allows dropping of Markdown documents into a folder to be rendered as HTML using a pre-configured HTML template.


### Version 3.0.15

* **Add Markdown TagHelper**  
Added a `<markdown />` TagHelper which allows embedding of static Markdown content into a page which is parsed into HTML at runtime. Also includes a Markdown Parser using `Markdown.Parse()` and `Markdown.ParseHtmlString()`. Uses the MarkDig Markdown Parser. Markdown features live in a separate NuGet package `Westwind.AspNetCore.Markdown`.

* **Add Markdown Parser**  
You can use the `Markdown.Parse()` and `Markdown.ParseHtmlString()` methods to render Markdown to HTML in code and Razor pages respectively.

* **AppUser Updates**   
Add additional functions to help with ClaimsPrincipal retrieval.


* **ErrorDisplay Tag Helper Updates**  
Fix Font-Awesome icon display for warning and errors. Fix `UnhandledApiExceptionFilter` bug with invalid declaration.