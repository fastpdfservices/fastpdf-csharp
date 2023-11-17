// -----------------------------------------------------------------
// PDFClient.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//     Represents a stylesheet file used in FastPDFService.
// -----------------------------------------------------------------


using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace FastPDFService.Models
{
    /// <summary>
    /// Represents a stylesheet file used in FastPDFService.
    /// </summary>
    /// <remarks>
    /// The StyleFile class encapsulates the properties of a stylesheet file, including its format,
    /// description, and associated data. It also includes metadata such as an identifier,
    /// timestamp, associated template ID, and filename.
    /// </remarks>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StyleFile
    {
        /// <summary>
        /// Gets or sets the format of the stylesheet file. Default is "css".
        /// </summary>
        public string? Format { get; set; } = "css";

        /// <summary>
        /// Gets or sets the description of the stylesheet file. Default is "fastpdf-csharp StyleFile".
        /// </summary>
        public string? Description { get; set; } = "fastpdf-csharp StyleFile";

        /// <summary>
        /// Gets or sets the binary data of the stylesheet file.
        /// </summary>
        public byte[]? StylesheetFile { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the stylesheet file.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional numerical value associated with the stylesheet file.
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the stylesheet file.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the template associated with the stylesheet file.
        /// </summary>
        public string? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the filename of the stylesheet file.
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleFile"/> class.
        /// </summary>
        public StyleFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleFile"/> class with the specified format.
        /// </summary>
        /// <param name="format">The format of the stylesheet file.</param>
        public StyleFile(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleFile"/> class with the specified format and description.
        /// </summary>
        /// <param name="format">The format of the stylesheet file.</param>
        /// <param name="description">The description of the stylesheet file.</param>
        public StyleFile(string format, string description)
            : this(format)
        {
            Description = description;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is StyleFile other)
            {
                return Format == other.Format &&
                       Description == other.Description &&
                       Equals(StylesheetFile, other.StylesheetFile) &&
                       Id == other.Id &&
                       Number == other.Number &&
                       Timestamp == other.Timestamp &&
                       TemplateId == other.TemplateId &&
                       Filename == other.Filename;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash1 = HashCode.Combine(Format, Description, StylesheetFile, Id, Number, Timestamp);
            var hash2 = HashCode.Combine(TemplateId, Filename);
            return HashCode.Combine(hash1, hash2);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var stylesheetFileString = StylesheetFile != null 
                ? BitConverter.ToString(StylesheetFile)[..Math.Min(20, StylesheetFile.Length)] + "..." 
                : "null";
            return $"StyleFile {{ Format = {Format}, Description = {Description}, " +
                   $"StylesheetFile = [{stylesheetFileString}], Id = {Id}, Number = {Number}, " +
                   $"Timestamp = {Timestamp}, TemplateId = {TemplateId}, Filename = {Filename} }}";
        }
    }
}
