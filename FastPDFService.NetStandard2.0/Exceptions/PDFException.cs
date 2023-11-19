// -----------------------------------------------------------------
// PDFException.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//   The PDFException class is a custom exception type for handling 
//   exceptions specific to PDF operations within the FastPDFService.
//   It provides additional context such as the HTTP status code and 
//   response text from the server, aiding in better exception handling
//   and debugging.
// -----------------------------------------------------------------


using System;

namespace FastPDFService.Exceptions
{
    /// <summary>
    /// Represents errors that occur during PDF processing operations.
    /// </summary>
    public class PDFException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the response text from the server, providing more details about the error.
        /// </summary>
        public string ResponseText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFException"/> class with a specific status code, error message, and response text.
        /// </summary>
        /// <param name="statusCode">The HTTP status code associated with the error.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="responseText">The detailed response text received from the server.</param>
        public PDFException(int statusCode, string message, string responseText)
            : base($"{message}. Status Code: {statusCode}, Response: {responseText}")
        {
            StatusCode = statusCode;
            ResponseText = responseText;
        }

    }
}
