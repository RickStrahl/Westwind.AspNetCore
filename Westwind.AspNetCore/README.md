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
A common base controller class that adds support for an auto-initialized BaseViewModel from which other VMs can inherit. Allows for automatic initialization of common features like ErrorDisplay and Base View models.

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

* ~~**User Token Manager**~~  
*Moved to Westwind.Utilities.Data*  
~~A database driven token manager that can create, store, validate and manage the life time of short lived generated tokens. Useful for creating tokens that are assigned after an initial authentication and then used for API access.~~


#### General ASP.NET Core

* **Custom Headers Middleware**  
Allows adding and removing of HTTP headers to every request using middleware configuration.

* **HttpRequest Extensions**  
    * `GetBodyStringAsync()` and `GetRawBodyBytesAsync()`  - retrieve raw non-JSON content
    * `MapPath()` - Map virtual path to physical path on disk
    * `Params()` - Return an item from Form, Query or Session collections.

* **DataProtector Wrapper**  
Helper to make it easier to use the DataProtector API to create secure tokens.

* **UserState Helper**  
The UserState object greatly simplifies working with auth 'cached' token data more easily by storing a single value in a user claim or forms auth ticket that can be easily restored into an typed object. The class supports easily serialization and auto-loading from Claims. Can be extended by subclassing and adding your own values. 


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
