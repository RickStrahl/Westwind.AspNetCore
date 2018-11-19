# Westwind.AspNetCore
#### Utility library providing useful helpers, formatters and extensions for ASP.NET Core

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/westwind.aspnetcore.svg)](https://www.nuget.org/packages?q=Westwind.aspnetcore)

This is a small helper package that provides a number of small helper and extension classes that facilitate common operations in ASP.NET Core and MVC applications.

> #### Westwind.AspnetCore.Markdown has moved
> The **Westwind.AspnetCore.Markdown** sub-component has moved to its own repository:
> 
> [Westwind.AspNetCore.Markdown on GitHub](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown)


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

#### Westwind.AspnetCore.Markdown Package
There is also a separate package for [Westwind.AspNetCore.Markdown](https://www.nuget.org/packages/Westwind.AspNetCore.Markdown) which provides a Markdown TagHelper and Markdown Parsing services:

```ps
PM> install-package westwind.aspnetcore.markdown
```

or the `dotnet` command line:

```ps
dotnet add package westwind.aspnetcore.markdown
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
    * `GetBodyStringAsync()` and `GetRawBodyBytesAsync()`  - retrieve raw non-JSON content
    * `MapPath()` - Map virtual path to physical path on disk
    * `Params()` - Return an item from Form, Query or Session collections.


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