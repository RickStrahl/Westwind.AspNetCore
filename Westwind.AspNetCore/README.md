![](icon.png)

General purpose support library for ASP.NET Core.

## MVC Functionality

* **BaseController and BaseViewModel implementation**
A common base controller class that adds support for an auto-initialized BaseViewModel from which other VMs can inherit. Allows for automatic initialization of common features like ErrorDisplay and Base View models.

* **ViewRenderer**
Render view output to a string using a controller context.

* **AppUser ClaimsPrincipal and Cookie Authentication Helper**
A `AppUser` class that wraps a `ClaimsPrincipal` and makes it easier to add and retrieve claims as well as easily login and logout all from a single helper object.

* **Bootstrap Alert ErrorDisplay Tag Helper and Controller Support Feature**
In most MVC applications you need some sort of error display and this ErrorDisplay TagHelper makes it quick easy to display an Alert box from a custom `ErrorDisplayModel` input. Helper methods like `ShowError()` or `ShowInfo()` on `BaseViewModel` make it very easy to display error and informational messages on pages.

## Api Functionality

* **Api Error Handling Filter**
A custom API error filter implementation that returns API responses on exceptions. Also provides a standardized `ApiExecption` class that can be used to force responses with specific HTTP response codes.

* **RawRequest Body String Formatter**
API formatter that allows for receiving raw non-json content to `string` and `byte[]` parameters, which otherwise isn't supported by MVC's API implementation. [More info in blog post](https://weblog.west-wind.com/posts/2017/Sep/14/Accepting-Raw-Request-Body-Content-in-ASPNET-Core-API-Controllers).


## General ASP.NET Core

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
   
* **User Token Manager**   
Class that generates a unique token that is valid for a given time period. Can be used for API authentication by validating an Authentication and then issuing a time released token that can be verified for validity.