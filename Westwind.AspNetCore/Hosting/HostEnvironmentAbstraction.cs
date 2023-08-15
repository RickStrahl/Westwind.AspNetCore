﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// A Hosting Environment Abstraction for ASP.NET Core that
    /// can be used to provide a single .Host instance that works
    /// for both .NET Core 3.x and 2.x
    ///
    /// Requires dual targeting for 2.x and 3.x
    /// </summary>
    /// <example>
    /// var hostAbstraction = new HostingAbstraction( app.ApplicationServices);
    /// app.AddSingleton<HostingAbstraction>(hostAbstraction);
    ///
    /// then either:
    /// 
    ///  * Use HostEnvironmentAbstraction.CurrentHost
    ///  * Or inject `HostEnvironmentAbstraction` with DI
    /// </example>
    [Obsolete("This abstraction is no longer needed post .NET 3.0 - Use IWebHostEnvironment directly from DI.")]
    public class HostEnvironmentAbstraction
    {
        public HostEnvironmentAbstraction(IServiceProvider provider)
        {
            if (CurrentHost == null)
                InitializeHost(provider);
        }

        /// <summary>
        /// Active Web Hosting Environment instance appropriate for the
        /// .NET version you're running.
        /// </summary>
        public static IWebHostEnvironment CurrentHost {get; set;}


        /// <summary>
        /// Active Web Hosting Environment instance appropriate for the
        /// .NET version you're running.
        /// </summary>
        public IWebHostEnvironment Host
        {
            get { return CurrentHost; }
        }


        /// <summary>
        /// Initializes the host by retrieving either IWebHostEnvironment or IHostingEnvironment
        /// from DI 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void InitializeHost(IServiceProvider serviceProvider)
        {
            CurrentHost = serviceProvider.GetService<IWebHostEnvironment>();
        }

    }

   
}
