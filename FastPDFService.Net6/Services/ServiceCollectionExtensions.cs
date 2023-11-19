// -----------------------------------------------------------------
// ServiceCollectionExtensions.cs
// Created on 19 Nov 2023 by K.Edeline
// -----------------------------------------------------------------
//
// Description:
// The ServiceCollectionExtensions.cs file contains extension methods 
// for IServiceCollection to simplify the integration of FastPDFService
// into .NET applications using dependency injection. This file
// primarily includes the AddPdfService method, which streamlines the
// process of configuring and registering the PDFService with the 
// dependency injection container.
//
// Usage:
// In an ASP.NET Core application, the AddPdfService extension method
// can be used in the ConfigureServices method of the Startup class
// or Program.cs (in .NET 6+). This method allows the user to configure
// PDFServiceOptions, such as setting the ApiKey, and registers the
// PDFService as an implementation of IPDFService. Once registered,
// IPdfService can be injected into controllers, services, or any other
// components that require PDF functionality.
//
// Get your API KEY at: https://fastpdfservice.com
// Documentation available at: https://docs.fastpdfservice.com
//
// Example:
// public void ConfigureServices(IServiceCollection services)
// {
//     services.AddPdfService(options =>
//     {
//         options.ApiKey = "your-api-key";
//     });
//
//     // Other service registrations...
// }
//
// Dependencies:
// - Microsoft.Extensions.DependencyInjection
// - Microsoft.Extensions.Options
//
// -------------------------------------------------------------------------------


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FastPDFService.Models;
using System;

namespace FastPDFService
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register FastPDFService-related services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures PDFService and its dependencies to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configure">An <see cref="Action{T}"/> to configure the provided <see cref="PDFServiceOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>
        /// This method simplifies the registration of PDFService into the dependency injection container.
        /// It allows configuring PDFServiceOptions such as ApiKey and ApiVersion via the provided delegate.
        /// After calling this method, IPdfService can be injected into any service or controller where PDF functionality is needed.
        /// </remarks>        
        public static IServiceCollection AddPdfService(this IServiceCollection services, Action<PDFServiceOptions> configure)
        {
            // Apply configuration to PDFServiceOptions
            services.Configure(configure);

            // Register PDFService with the DI container
            services.AddScoped<IPDFService, PDFService>();

            return services;
        }
    }
}
