using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;


namespace Westwind.AspNetCore;

/// <summary>
/// Runs after the full route table is built. Removes descriptors for base
/// controller actions when a subclass is also registered, eliminating the
/// ambiguous-route conflict (duplicate routes).
///
/// Register in Program.cs:
///   var convention = new InheritedControllerRouteConvention();
///   services.AddSingleton&lt;IActionDescriptorProvider&gt;(convention);
/// </summary>
public class InheritedControllerRouteConvention :  IActionDescriptorProvider
{
 
    /// <summary>
    /// Optionally restrict which inherited concrete controller base types 
    /// we want to allow base class routes to work on.
    /// If empty, all inherited concrete controller base types are processed.    
    /// </summary>
    public List<Type> ChildControllerTypes { get; set; } = [];


    public int Order => 0;

    public void OnProvidersExecuting(ActionDescriptorProviderContext context) { }


    /// <summary>
    /// This method finds all base controller types or those of the type(s)
    /// specified in <see cref="ChildControllerTypes"/>
    /// and removes any [Route()] attributes on the child controllers
    /// to fix the duplication of routes that break inherited controller routing.
    /// </summary>
    /// <param name="context"></param>
    public void OnProvidersExecuted(ActionDescriptorProviderContext context)
    {
        var descriptors = context.Results
            .OfType<ControllerActionDescriptor>()
            .ToList();

        var controllerTypes = descriptors
            .Select(d => d.ControllerTypeInfo.AsType())
            .Distinct()
            .ToList();

        // Find child types to process (all, or the restricted list)
        var childTypes = ChildControllerTypes.Count > 0
            ? controllerTypes.Where(t => ChildControllerTypes.Contains(t)).ToList()
            : controllerTypes;

        // Base types are those that a processed child inherits from and that are
        // also directly registered as controllers
        var baseTypesToSuppress = controllerTypes
            .Where(t => childTypes.Any(child => IsSubclassOf(child, t)))
            .ToList();

        var toRemove = descriptors
            .Where(d => baseTypesToSuppress.Any(bt => d.ControllerTypeInfo.AsType() == bt))
            .ToList();

        foreach (var d in toRemove)
            context.Results.Remove(d);
    }

    private static bool IsSubclassOf(Type child, Type parent) => child != parent && parent.IsAssignableFrom(child);

}
