# Westwind.AspNetCore
#### Utility library providing useful helpers, formatters and extensions for ASP.NET Core

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/westwind.aspnetcore.svg)](https://www.nuget.org/packages?q=Westwind.aspnetcore)

This is a small helper package that provides a number of small helper and extension classes that facilitate common operations in ASP.NET Core and MVC applications.

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

### Features

#### MVC Functionality

* **BaseController and BaseViewModel implementation**  
A common base controller class that adds support for an auto-initialized BaseViewModel from which other VMs can inherit. Allows for automatic initialization of common features like ErrorDisplay and Base View models. Also optionally sets up a UserState object that can be used to persist user data (display name, stats, etc) across requests.

* **ViewRenderer**  
Render Razor/MVC view output to a string using a controller context.

* **FormVariable to Object Instance Binder**  
`Request.Form` unbind routine that allows you to unbind form variables into an existing object only updating properties that are are available in the request form context.

* **AppUser ClaimsPrincipal and Cookie Authentication Helper**  
A `AppUser` class that wraps a `ClaimsPrincipal` and makes it easier to add and retrieve claims as well as easily login and logout all from a single helper object.

* **Bootstrap Alert ErrorDisplay Tag Helper and Controller Support Feature**  
In most MVC applications you need some sort of error display and this ErrorDisplay TagHelper makes it quick easy to display an Alert box from a custom `ErrorDisplayModel` input. Helper methods like `ShowError()` or `ShowInfo()` on `BaseViewModel` make it very easy to display error and informational messages on pages.

#### Api Functionality

* **Api Error Handling Filter**  
A custom API error filter implementation that returns API responses on exceptions. Also provides a standardized `ApiExecption` class that can be used to force responses with specific HTTP response codes.

* **Api Base Response Object**  
`ApiResponse` base class that can be used to return consistent API results that include error status, error message, status code as well as the actual data. Both typed and untyped versions.

* **RawRequest Body String Formatter**   
API formatter that allows for receiving raw non-json content to `string` and `byte[]` parameters, which otherwise isn't supported by MVC's API implementation. [More info in blog post](https://weblog.west-wind.com/posts/2017/Sep/14/Accepting-Raw-Request-Body-Content-in-ASPNET-Core-API-Controllers).

* **User Token Manager**  
A database driven token manager that can create, store, validate and manage the life time of short lived generated tokens. Useful for creating tokens that are assigned after an initial authentication and then used for API access and can be easily validated.

* **JWT Helper**  
Helper class that makes it easier to create and retrieve JWT tokens.


#### General ASP.NET Core

* **Custom Headers Middleware**  
Allows adding and removing of HTTP headers to every request using middleware configuration.

* **HttpRequest Extensions**  
    * `GetBodyStringAsync()` and `GetRawBodyBytesAsync()`  - retrieve raw non-JSON content
    * `GetUrl()` - Returns the Absolute URL for the current request.
    * `Params()` - Return an item from Form, Query or Session collections.
    * `IsFormVar()` -  Determines if a Form variable exists
    * `IsPostback()` - Determines if request is a Post/Put operation
    * `IsLocal` - Determines if the current URL is a local machine URL
    
* **HttpContext Extensions**
    * `MapPath()` - Map virtual path to physical path on disk

* **DataProtector Wrapper**  
Helper to make it easier to use the DataProtector API to create secure tokens.

* **UserState Helper**  
The UserState object greatly simplifies working with auth 'cached' user data that can be stored across requests. Useful for caching things like username, main IDs to reduce data base lookups or simply to carry global values across requests. Data is stored either in an Identity Claim or a custom encrypted cookie. The class supports easily serialization and auto-loading and saving. Can be extended by subclassing and adding your own custom properties to track beyond several common ones.


## License
The Westwind.Web.MarkdownControl library is an open source product licensed under:

* **[MIT license](http://opensource.org/licenses/MIT)**

All source code is **&copy; West Wind Technologies**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact if present.

There's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application and you are free to modify the code for use in your own projects.

### Give back
If you find this library useful, consider making a small donation using the Sponsor link.