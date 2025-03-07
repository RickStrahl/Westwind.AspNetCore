# Westwind.AspNetCore
#### Utility library providing useful helpers, formatters and extensions for ASP.NET Core

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/westwind.aspnetcore.svg)](https://www.nuget.org/packages?q=Westwind.aspnetcore)

![](icon.png)

This is a small package that provides a number of convenience helper and extension classes that facilitate common operations in ASP.NET Core API and MVC applications and ASP.NET Core in general.

You can use this package as is or use the source code to pick and choose pieces to add to your own applications - most features are self-contained and easily moved and integrated.

### Installation
You can install the package [from NuGet](https://www.nuget.org/packages/Westwind.AspNetCore/) in Visual Studio:

#### Westwind.AspnetCore Package

```ps
PM> install-package westwind.aspnetcore
```

or the `dotnet` command line:

```ps
dotnet add package westwind.aspnetcore
```

### Documentation
A class reference for the library can be found here:

* [Westwind.AspNetCore Class Reference Documenation](https://docs.west-wind.com/westwind.aspnetcore)

### Features

#### General ASP.NET Functionality

* **FormVariableBinding unbinds forms to an Object**  
`Request.Form` unbind routine that allows you to unbind form variables into an existing object only updating properties that are are available in the request form context. This can in many instances be much easier to use than Model binding which creates new objects and so requires tracking changes.

* **Custom Http Headers Middleware**  
Allows adding and removing of HTTP headers to every request using middleware configuration. It's a missing piece in the ASP.NET Core base libraries.

* **HttpRequest Extensions**  
    * `GetBodyStringAsync()` and `GetRawBodyBytesAsync()`  - retrieve raw non-JSON content
    * `GetUrl()` - Returns the Absolute URL for the current request.
    * `Params()` - Return an item from Form, Query or Session collections.
    * `IsFormVar()` -  Determines if a Form variable exists
    * `IsPostback()` - Determines if request is a Post/Put operation
    * `IsLocal` - Determines if the current URL is a local machine URL
    * `UnbindFormVars()` - unbinds form vars into an object (uses FormVariable Binder)
    
* **HttpContext Extensions**
    * `MapPath()` - Map virtual path to physical path on disk
    * `ResolvePath()` - resolves a relative path to a fully qualified site path (~/, /)
    * `SetUserLocale()` - lets you easily set User locale and UiLocale for a request

* **DataProtector Wrapper**  
Helper to make it easier to use the DataProtector API to create secure tokens.

* **UserState Helper**  
The UserState object greatly simplifies working with auth 'cached' user data that can be stored across requests. Useful for caching things like username, main IDs to reduce data base lookups or simply to carry global values across requests. Data is stored either in an Identity Claim or a custom encrypted cookie. The class supports easily serialization and auto-loading and saving. Can be extended by subclassing and adding your own custom properties to track beyond several common ones.

* **WeUtils** 
  * `EncodeJsString()` - string encoding for Json without library requirements
  * `DecodeJsString()` - string decoding for Json without library requirements
  * `EncodeJsDate()` - data encoding for Json without library requirements

* **Gravatar**  
`Gravatar` class that quickly lets you embed a `GravatarLink()` and `GravatarImage()` into pages.

#### MVC Functionality

* **BaseController and BaseViewModel implementation**  
A common base controller class that adds support for an auto-initialized BaseViewModel from which other VMs can inherit. Allows for automatic initialization of common features like ErrorDisplay and Base View models.

* **ViewRenderer**  
Render Razor/MVC view output to a string using a controller context.


* **AppUser ClaimsPrincipal and Cookie Authentication Helper**  
A `AppUser` class that wraps a `ClaimsPrincipal` and makes it easier to add and retrieve claims as well as easily login and logout all from a single helper object.

* **Razor Error Display Component based on Bootstrap Alert**  
Provides an `<error-display>` razor component that you model bind directly, or you can use the built in `ErrorDisplayModel` on the `BaseViewModel` class in overloaded ViewModel components of your own.

  In most MVC applications you need some sort of error display and this ErrorDisplay TagHelper makes it quick easy to display an Alert box from a custom `ErrorDisplayModel` input. Helper methods like `ShowError()`, `ShowSuccess()` or `ShowInfo()` on `BaseViewModel` make it very easy to display error and informational messages on pages, consistently

#### Api Functionality

* **BaseApiController** 
API base class that includes exception trapping and display and can be used with UserState that is passed through requests from Auth tokens or other custom auth mechanism. Similar to BaseControl but optimized for API operation.

* **ApiExceptionFilter Error Handling and ApiException**  
A custom API error filter implementation that returns JSON API responses on exceptions. Also provides a standardized `ApiExecption` class that can be used to easily throw exceptions that returns specific HTTP response codes.

* **Api Base Response Object**  
`ApiResponse` base class that can be used to return consistent API results that include error status, error message, status code as well as the actual data. Both typed and untyped versions.

* **RawRequest Body String Formatter**   
API formatter that allows for receiving raw non-json content to `string` and `byte[]` parameters, which otherwise isn't supported by MVC's API implementation. [More info in blog post](https://weblog.west-wind.com/posts/2017/Sep/14/Accepting-Raw-Request-Body-Content-in-ASPNET-Core-API-Controllers).

* ~~**User Token Manager**~~  
*Moved to Westwind.Utilities.Data*  
~~A database driven token manager that can create, store, validate and manage the life time of short lived generated tokens. Useful for creating tokens that are assigned after an initial authentication and then used for API access.~~

* **JWT Helper** 
Make it easier to create JWT Tokens in the ASP.NET Auth configuration


#### General ASP.NET Core


## License
The Westwind.Web.MarkdownControl library is an open source product licensed under:

* **[MIT license](http://opensource.org/licenses/MIT)**

All source code is **&copy; West Wind Technologies**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact if present.

There's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application and you are free to modify the code for use in your own projects.

### Give back
If you find this library useful, consider making a small donation:

<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BA3NHHFHTMXD8" 
    title="Find this library useful? Consider making a small donation." alt="Make Donation" style="text-decoration: none;">
	<img src="https://weblog.west-wind.com/images/donation.png" />
</a>


### Update History

* **Version 4.0**  
Physically removed UserTokenManager from the package (see `v3.20` for initial deprecation).

* **Version 3.20**  
Removed the UserTokenManager class from this package and moved it into Westwind.Utilities.Data in order to remove the default footprint for the SQL libraries from this package. 

#### Breaking Changes
The structure of the UserTokens table for `UserTokenManager` has changed with some additional fields. The table has to be updated to include additional fields. If your DB has write access for the connection string you can delete the table and let it rebuild. Otherwise look at the `UserTokenManager.CreateUserTokenSqlTable()` method for the latest structure and SQL statement.
