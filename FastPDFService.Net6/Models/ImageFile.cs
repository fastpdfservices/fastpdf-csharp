// -----------------------------------------------------------------
// PDFClient.cs
// Created on 15 Nov 2023 by K.Edeline
//
// Description:
//     Represents an image file used in FastPDFService.
// -----------------------------------------------------------------


using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace FastPDFService.Models
{
    /// <summary>
    /// Represents an image file used in FastPDFService.
    /// </summary>
    /// <remarks>
    /// The ImageFile class encapsulates the properties of an image file, including its format,
    /// URI, description, and associated data. It also includes metadata such as an identifier,
    /// a numerical value, timestamp, associated template ID, and filename.
    /// </remarks>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ImageFile
    {
        /// <summary>
        /// Gets or sets the format of the image file.
        /// </summary>
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets the URI of the image file.
        /// </summary>
        public string? Uri { get; set; }

        /// <summary>
        /// Gets or sets the description of the image file. Default is "fastpdf-csharp ImageFile".
        /// </summary>
        public string? Description { get; set; } = "fastpdf-csharp ImageFile";

        /// <summary>
        /// Gets or sets the binary data of the image file.
        /// </summary>
        public byte[]? ImageFileData { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the image file.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional numerical value associated with the image file.
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the image file.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the template associated with the image file.
        /// </summary>
        public string? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the filename of the image file.
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFile"/> class.
        /// </summary>
        public ImageFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFile"/> class with the specified format.
        /// </summary>
        /// <param name="format">The format of the image file.</param>
        public ImageFile(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFile"/> class with the specified format and URI.
        /// </summary>
        /// <param name="format">The format of the image file.</param>
        /// <param name="uri">The URI of the image file.</param>
        public ImageFile(string format, string uri)
            : this(format)
        {
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFile"/> class with the specified format, URI, and description.
        /// </summary>
        /// <param name="format">The format of the image file.</param>
        /// <param name="uri">The URI of the image file.</param>
        /// <param name="description">The description of the image file.</param>
        public ImageFile(string format, string uri, string description)
            : this(format, uri)
        {
            Description = description;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is ImageFile other)
            {
                return Format == other.Format &&
                    Uri == other.Uri &&
                    Description == other.Description &&
                    Equals(ImageFileData, other.ImageFileData) && // For byte array comparison
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
            var hash1 = HashCode.Combine(Format, Uri, Description, ImageFileData, Id, Number, Timestamp);
            var hash2 = HashCode.Combine(TemplateId, Filename);
            return HashCode.Combine(hash1, hash2);
        }
        
        /// <inheritdoc/>
        public override string ToString()
        {
            var templateFileData = ImageFileData != null 
                ? BitConverter.ToString(ImageFileData)[..Math.Min(20, ImageFileData.Length)] + "..." 
                : "null";
            return $"ImageFile {{ Format = {Format}, Uri = {Uri}, Description = {Description}, " +
                $"ImageFileData = [{templateFileData}], Id = {Id}, Number = {Number}, Timestamp = {Timestamp}, " +
                $"TemplateId = {TemplateId}, Filename = {Filename} }}";
        }
    }
}
