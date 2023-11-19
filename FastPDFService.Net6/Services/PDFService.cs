// -----------------------------------------------------------------
// PDFService.cs
// Created on 19 Nov 2023 by K.Edeline
// -----------------------------------------------------------------
//
// Description:
// The PDFService class serves as a service layer on top of the PDFClient.
// It provides an implementation of the IPDFService interface, making it
// suitable for dependency injection in .NET applications. This class
// simplifies interactions with the underlying PDFClient by handling
// configuration and initialization processes.
//
// The class requires an API key for instantiation, which is provided
// through the PDFServiceOptions class. This design allows for flexible
// configuration, especially when integrating with ASP.NET Core projects.
//
// Usage:
// The PDFService can be easily registered with the ASP.NET Core's
// dependency injection system. Users need to provide the API key and
// other optional settings via the PDFServiceOptions class.
//
// Get your API KEY at: https://fastpdfservice.com
// Documentation available at: https://docs.fastpdfservice.com
//
//
// Example:
// services.AddPdfService(options =>
// {
//     options.ApiKey = "your-api-key";
// });
//
// After registration, IPDFService can be injected into controllers,
// services, or other components that require PDF functionality.
//
// Dependencies:
// - Microsoft.Extensions.Options
// - FastPDFService.Models
//
// -------------------------------------------------------------------------------


using Microsoft.Extensions.Options;
using FastPDFService.Models;
using System;

namespace FastPDFService
{

    /// <summary>
    /// Configuration options for PDFService.
    /// </summary>
    public class PDFServiceOptions
    {
        /// <summary>
        /// Gets or sets the API key used for authenticating with the PDF service. 
        /// It is mandatory to provide an API key for the PDFService to function properly. 
        /// The API key can be obtained from the FastPDF dashboard at https://fastpdfservice.com 
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// Gets or sets the optional API version.
        /// </summary>
        public string? ApiVersion { get; set; }
    }

    /// <summary>
    /// PDFService provides a service layer over the PDFClient,
    /// implementing the IPdfService interface for use in .NET applications.
    /// </summary>
    public class PDFService : PDFClient, IPDFService 
    {
        /// <summary>
        /// Initializes a new instance of the PDFService with configuration options.
        /// </summary>
        /// <param name="options">Configuration options for PDFService.</param>
        /// <exception cref="ArgumentException">Thrown if API key is not provided.</exception>
        public PDFService(IOptions<PDFServiceOptions> options)
            : base(options.Value.ApiKey)
        {
            if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
            {
                throw new ArgumentException("API key is required for PDFService.");
            }
        }
    }
}
