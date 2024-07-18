# Westwind.AspNetCore Change Log
<small>[Nuget](https://www.nuget.org/packages/Westwind.AspNetCore/) &bull; [Github](https://github.com/RickStrahl/Westwind.AspNetCore)</small>


### Version 3.9.2

* **Add TokenIdentifier to UserTokenManager**  
Add an extra field and retrieval mechanism to retrieve a token based on a separate token. This can be useful in local/desktop authentication scenarios where you provide a token identifier that you can query for via an API until the auth operation is complete.

* **Fix: UserState.IsAdmin**  
UserState IsAdmin flag now explicitly checks for authentication in addition to the flags value, when returning its state to avoid potential issues with multi-factor authentication where flag may be set before two-factor has completed.

### Version 3.8

* **Moved HttpRequestExtensions.MapPath() to HttpContextExtensions.MapPath()**  
Moved `MapPath()`  function to new `HttpContextExtensions` to allow easier access in non request scenarios. `MapPath()` is available during startup and so can be used for middleware configuration.

* **Refactor BaseController, BaseViewModel and UserState for easier Integration**  
Base controller can now automatically be set up for handling custom userstate objects by providing the generic type overload removing the need to create custom overrides of the `Initialize()` and `OnActionExecuted()` in implemented controllers. A new `UserStateSettings` object also allows global configuration and it now supports both plain Cookie and Identity Claims for storing user state.


### Version 3.7

* **Add .NET 7.0 Target**  
Added support for .NET 7.0 and reset targeting for `net60;net70;net50`.


### Version 3.6

* **Add explicit support for .NET 6.0**  
Added support for .NET 6.0 and reset targeting for `net60;net50;netcoreapp3.1`.

* **Form Variable Binder Unbinding to existing Objects**  
Added FormVariableBinder class that unbinds `Request.Form` data to an existing object. Supports flat, single-level unbinding, but supports prefixes to allow for child object unbinding as well as property exclusions and explicit inclusions.

* **Add SQL based UserTokenService for short lived Token Generation**   
This database service lets you generate, store, validate and manage the lifetime of short lived tokens often used for Bearer tokens. Useful for scenarios where you sign in first to generate a token which can then be used for subsequent request on an API which can then validate the token.

* **Add base ApiResponse Base Class**  
Added a base ApiResponse class that can be used to return consistent API messages back to API clients. Message includes Message, Status and Data fields where data contains the payload. Both typed and untyped versions.



### Version 3.5

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