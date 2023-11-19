// -----------------------------------------------------------------
// Template.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//     Represents a template used in FastPDFService.
// -----------------------------------------------------------------


using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FastPDFService.Models
{

    /// <summary>
    /// Represents a template used in FastPDFService.
    /// </summary>
    /// <remarks>
    /// The Template class encapsulates properties related to a document template, including its name,
    /// format, description, associated files, and various settings for rendering documents.
    /// </remarks>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Template
    {
        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the format of the template. Default is "html".
        /// </summary>
        public string Format { get; set; } = "html";

        /// <summary>
        /// Gets or sets the description of the template. Default is "fastpdf-csharp Template".
        /// </summary>
        public string Description { get; set; } = "fastpdf-csharp Template";

        /// <summary>
        /// Gets or sets the unique identifier of the template.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the template.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the main template file.
        /// </summary>
        public TemplateFile TemplateFile { get; set; }

        /// <summary>
        /// Gets or sets the header file for the template.
        /// </summary>
        public TemplateFile HeaderFile { get; set; }

        /// <summary>
        /// Gets or sets the footer file for the template.
        /// </summary>
        public TemplateFile FooterFile { get; set; }

        /// <summary>
        /// Gets or sets the array of image files associated with the template.
        /// </summary>
        public ImageFile[] ImageFiles { get; set; }

        /// <summary>
        /// Gets or sets the array of stylesheet files associated with the template.
        /// </summary>
        public StyleFile[] StyleFiles { get; set; }

        /// <summary>
        /// Gets or sets whether the template should be rendered in landscape orientation.
        /// </summary>
        public bool? Landscape { get; set; }

        /// <summary>
        /// Gets or sets the paper format for the template.
        /// </summary>
        public string PaperFormat { get; set; }

        /// <summary>
        /// Gets or sets whether to print background graphics in the rendered document.
        /// </summary>
        public bool? PrintBackground { get; set; }

        /// <summary>
        /// Gets or sets the page range for rendering. Defines specific pages or ranges to render.
        /// </summary>
        public string PageRange { get; set; }

        /// <summary>
        /// Gets or sets the scale factor for rendering the template.
        /// </summary>
        public float? Scale { get; set; }

        /// <summary>
        /// Gets or sets the top margin for the rendered document.
        /// </summary>
        public float? MarginTop { get; set; }

        /// <summary>
        /// Gets or sets the right margin for the rendered document.
        /// </summary>
        public float? MarginRight { get; set; }

        /// <summary>
        /// Gets or sets the bottom margin for the rendered document.
        /// </summary>
        public float? MarginBottom { get; set; }

        /// <summary>
        /// Gets or sets the left margin for the rendered document.
        /// </summary>
        public float? MarginLeft { get; set; }

        /// <summary>
        /// Gets or sets whether the page number footer is enabled in the rendered document.
        /// </summary>
        public bool? PageNumberFooterEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the title header is enabled in the rendered document.
        /// </summary>
        public bool? TitleHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the date header is enabled in the rendered document.
        /// </summary>
        public bool? DateHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the header and footer are disabled on the first page of the rendered document.
        /// </summary>
        public bool? DisableHeaderFooterFirstPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        public Template() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class with a specified name.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        public Template(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class with a specified name and format.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="format">The format of the template, typically "html".</param>
        public Template(string name, string format)
            : this(name)
        {
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class with a specified name, format, and description.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="format">The format of the template, typically "html".</param>
        /// <param name="description">A description of the template.</param>
        public Template(string name, string format, string description)
            : this(name, format)
        {
            Description = description;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Template other)
            {
                return Name == other.Name &&
                    Format == other.Format &&
                    Description == other.Description &&
                    Id == other.Id &&
                    Timestamp == other.Timestamp &&
                    Equals(TemplateFile, other.TemplateFile) &&
                    Equals(HeaderFile, other.HeaderFile) &&
                    Equals(FooterFile, other.FooterFile) &&
                    Equals(ImageFiles, other.ImageFiles) &&
                    Equals(StyleFiles, other.StyleFiles) &&
                    Landscape == other.Landscape &&
                    PaperFormat == other.PaperFormat &&
                    PrintBackground == other.PrintBackground &&
                    PageRange == other.PageRange &&
                    Scale == other.Scale &&
                    MarginTop == other.MarginTop &&
                    MarginRight == other.MarginRight &&
                    MarginBottom == other.MarginBottom &&
                    MarginLeft == other.MarginLeft &&
                    PageNumberFooterEnabled == other.PageNumberFooterEnabled &&
                    TitleHeaderEnabled == other.TitleHeaderEnabled &&
                    DateHeaderEnabled == other.DateHeaderEnabled &&
                    DisableHeaderFooterFirstPage == other.DisableHeaderFooterFirstPage;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine
            {
                int hash = 17;
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + (Format?.GetHashCode() ?? 0);
                hash = hash * 23 + (Description?.GetHashCode() ?? 0);
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Timestamp.GetHashCode();
                hash = hash * 23 + (TemplateFile?.GetHashCode() ?? 0);
                hash = hash * 23 + (HeaderFile?.GetHashCode() ?? 0);
                hash = hash * 23 + (FooterFile?.GetHashCode() ?? 0);
                hash = hash * 23 + (ImageFiles?.GetHashCode() ?? 0);
                hash = hash * 23 + (StyleFiles?.GetHashCode() ?? 0);
                hash = hash * 23 + Landscape.GetHashCode();
                hash = hash * 23 + (PaperFormat?.GetHashCode() ?? 0);
                hash = hash * 23 + PrintBackground.GetHashCode();
                hash = hash * 23 + (PageRange?.GetHashCode() ?? 0);
                hash = hash * 23 + Scale.GetHashCode();
                hash = hash * 23 + MarginTop.GetHashCode();
                hash = hash * 23 + MarginRight.GetHashCode();
                hash = hash * 23 + MarginBottom.GetHashCode();
                hash = hash * 23 + MarginLeft.GetHashCode();
                hash = hash * 23 + PageNumberFooterEnabled.GetHashCode();
                hash = hash * 23 + TitleHeaderEnabled.GetHashCode();
                hash = hash * 23 + DateHeaderEnabled.GetHashCode();
                hash = hash * 23 + DisableHeaderFooterFirstPage.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Template {{ Name = {Name}, Format = {Format}, Description = {Description}, " +
                $"Id = {Id}, Timestamp = {Timestamp}, TemplateFile = {TemplateFile}, " +
                $"HeaderFile = {HeaderFile}, FooterFile = {FooterFile}, ImageFiles = {ImageFiles}, " +
                $"StyleFiles = {StyleFiles}, Landscape = {Landscape}, PaperFormat = {PaperFormat}, " +
                $"PrintBackground = {PrintBackground}, PageRange = {PageRange}, Scale = {Scale}, " +
                $"MarginTop = {MarginTop}, MarginRight = {MarginRight}, MarginBottom = {MarginBottom}, " +
                $"MarginLeft = {MarginLeft}, PageNumberFooterEnabled = {PageNumberFooterEnabled}, " +
                $"TitleHeaderEnabled = {TitleHeaderEnabled}, DateHeaderEnabled = {DateHeaderEnabled}, " +
                $"DisableHeaderFooterFirstPage = {DisableHeaderFooterFirstPage} }}";
        }
    }
}
