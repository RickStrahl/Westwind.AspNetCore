using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;


namespace Westwind.AspNetCore;

/// <summary>
/// Convention that allows routes to be inherited from base controller classes.
///
/// By default ASP.NET Core does not inherit route attributes from base classes,
/// so this convention walks the controller actions and adds route selectors for
/// any inherited route attributes.
/// </summary>
public class InheritedControllerRouteConvention : IApplicationModelConvention
{
    /// <summary>
    /// Optionally lets you specify the parent controller types on which you
    /// want inherited routes to apply to.
    /// </summary>
    public List<Type> ParentControllerTypes { get; set; } = [];

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            // if provided only check the controllers that are derived
            if (ParentControllerTypes.Count > 0 && !ParentControllerTypes.Contains(controller.ControllerType.AsType()))
                continue;

            // Walk every action on this controller
            foreach (var action in controller.Actions)
            {
                // Is this method declared on a base class, not the concrete controller?
                bool isInherited = action.ActionMethod.DeclaringType != controller.ControllerType.AsType();

                if (!isInherited)
                    continue;

                // Look for custom route attributes on the *base* method
                // (the ones the framework skipped because of the DeclaringType check)
                var routeAttributes = action.ActionMethod
                    .GetCustomAttributes(inherit: true)  // <-- inherit: true is key
                    .OfType<IRouteTemplateProvider>()
                    .ToList();

                if (!routeAttributes.Any())
                    continue;

                // If the action has no selectors with routes, add them from base class attrs
                var hasExistingRoute = action.Selectors
                    .Any(s => s.AttributeRouteModel != null);

                if (hasExistingRoute)
                    continue; // already wired up, leave it alone

                // Clear empty selectors and add one per route attribute found
                action.Selectors.Clear();
                foreach (var routeAttr in routeAttributes)
                {
                    var selector = new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(routeAttr)
                    };

                    // Copy HTTP method constraints from verb attributes
                    if (routeAttr is IActionHttpMethodProvider httpProvider)
                    {
                        foreach (var method in httpProvider.HttpMethods)
                            selector.ActionConstraints.Add(
                                new HttpMethodActionConstraint(new[] { method }));
                    }

                    action.Selectors.Add(selector);
                }
            }
        }
    }
}
