# Westwind.AspNetCore

#### ASP.NET Core Utility library that provides helpers, formatters and extensions for ASP.NET Core

This is a small helper package that provides a number of small helper and extension classes that facilitate common operations in ASP.NET Core and MVC applications.

### Installation
You can install the package [from NuGet](https://www.nuget.org/packages/Westwind.AspNetCore/):


```
PM> install-package westwind.aspnetcore
```

or the `dotnet` command line:

```
dotnet add package westwind.aspnetcore
```

### Features

#### Api Functionality

* **Api Error Handling Filter**  
A custom API error filter implementation that returns API responses on exceptions. Also provides a standardized `ApiExecption` class that can be used to force responses with specific HTTP response codes.

* **RawRequest Body String Formatter**   
API formatter that allows for receiving raw non-json content to `string` and `byte[]` parameters, which otherwise isn't supported by MVC's API implementation. [More info in blog post](https://weblog.west-wind.com/posts/2017/Sep/14/Accepting-Raw-Request-Body-Content-in-ASPNET-Core-API-Controllers).

#### MVC Functionality

* **BaseController and BaseViewModel implementation**  
A common base controller class that adds support for an auto-initialized BaseViewModel from which other VMs can inherit. Allows for automatic initialization of common features like ErrorDisplay or 

* **AppUser ClaimsPrincipal and Cookie Authentication Helper**  
A `AppUser` class that wraps a `ClaimsPrincipal` and makes it easier to add and retrieve claims as well as easily login and logout all from a single helper object.

* **Bootstrap Alert ErrorDisplay Tag Helper and Controller Support Feature**  
In most MVC applications you need some sort of error display and this ErrorDisplay TagHelper makes it quick easy to display an Alert box from a custom `ErrorDisplayModel` input. Helper methods like `ShowError()` or `ShowInfo()` on `BaseViewModel` make it very easy to display error and informational messages on pages.

#### General ASP.NET Core

* **HttpRequest Extensions**  
Helpers for:
* `GetBodyStringAsync()` and `GetRawBodyBytesAsync()`  - retrieve raw non-JSON content
* `MapPath()` - Map virtual path to physical path on disk
* `Params()` - Return an item from Form, Query or Session collections.


