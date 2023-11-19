// -----------------------------------------------------------------
// RenderOptions.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//   Defines options for rendering documents in the FastPDFService service,
//   including layout, format, margins, and other rendering parameters.
// -----------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FastPDFService.Models
{
    /// <summary>
    /// Represents rendering options for documents in the FastPDFService service,
    /// including layout, format, margins, and other rendering parameters.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RenderOptions
    {
        /// <summary>
        /// Gets or sets the templating engine to be used for rendering.
        /// </summary>
        public string TemplatingEngine { get; set; }

        /// <summary>
        /// Gets or sets the rendering engine.
        /// </summary>
        public string RenderingEngine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display header and footer.
        /// </summary>
        public bool? DisplayHeaderFooter { get; set; }

        /// <summary>
        /// Gets or sets the byte array of the header file.
        /// </summary>
        public byte[] HeaderFile { get; set; }

        /// <summary>
        /// Gets or sets the byte array of the footer file.
        /// </summary>
        public byte[] FooterFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the orientation is landscape.
        /// </summary>
        public bool? Landscape { get; set; }

        /// <summary>
        /// Gets or sets the paper format (e.g., "A4", "Letter").
        /// </summary>
        public string PaperFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the background should be rendered.
        /// </summary>
        public bool? Background { get; set; }

        /// <summary>
        /// Gets or sets the page range for rendering.
        /// </summary>
        public string PageRange { get; set; }

        /// <summary>
        /// Gets or sets the scale for rendering.
        /// </summary>
        public float? Scale { get; set; }

        /// <summary>
        /// Gets or sets the top margin.
        /// </summary>
        public float? MarginTop { get; set; }

        /// <summary>
        /// Gets or sets the right margin.
        /// </summary>
        public float? MarginRight { get; set; }

        /// <summary>
        /// Gets or sets the bottom margin.
        /// </summary>
        public float? MarginBottom { get; set; }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        public float? MarginLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page number footer is enabled.
        /// </summary>
        public bool? PageNumberFooterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title header is enabled.
        /// </summary>
        public bool? TitleHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the date header is enabled.
        /// </summary>
        public bool? DateHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets the X-coordinate for positioning elements.
        /// </summary>
        public float? X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate for positioning elements.
        /// </summary>
        public float? Y { get; set; }

        /// <summary>
        /// Gets or sets the width for elements (alias for Width).
        /// </summary>
        public float? W { get; set; }

        /// <summary>
        /// Gets or sets the height for elements (alias for Height).
        /// </summary>
        public float? H { get; set; }

        /// <summary>
        /// Gets or sets the width for elements.
        /// </summary>
        public float? Width { get; set; }

        /// <summary>
        /// Gets or sets the height for elements.
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the aspect ratio should be maintained.
        /// </summary>
        public bool? KeepRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether text is enabled in the rendering (e.g., barcode rendering).
        /// </summary>
        public bool? TextEnabled { get; set; }

        /// <summary>
        /// Gets or sets the image mode for rendering.
        /// </summary>
        public string ImageMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether compression is enabled.
        /// </summary>
        public bool? Compress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether transparency is enabled.
        /// </summary>
        public bool? TransparencyEnabled { get; set; }

        /// <summary>
        /// Gets or sets the background color in RGB format.
        /// </summary>
        public int[] BackgroundColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOptions"/> class.
        /// </summary>
        public RenderOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOptions"/> class with specified width and height.
        /// </summary>
        /// <param name="w">The width of the rendering area.</param>
        /// <param name="h">The height of the rendering area.</param>
        public RenderOptions(float w, float h)
        {
            W = w;
            H = h;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOptions"/> class with specified width, height, and margins.
        /// </summary>
        /// <param name="w">The width of the rendering area.</param>
        /// <param name="h">The height of the rendering area.</param>
        /// <param name="marginTop">The margin at the top of the rendering area.</param>
        /// <param name="marginBottom">The margin at the bottom of the rendering area.</param>
        /// <param name="marginLeft">The margin on the left side of the rendering area.</param>
        /// <param name="marginRight">The margin on the right side of the rendering area.</param>
        public RenderOptions(float w, float h, float marginTop, float marginBottom, float marginLeft, float marginRight)
            : this(w, h)
        {
            MarginTop = marginTop;
            MarginBottom = marginBottom;
            MarginLeft = marginLeft;
            MarginRight = marginRight;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is RenderOptions other)
            {
                return TemplatingEngine == other.TemplatingEngine &&
                    RenderingEngine == other.RenderingEngine &&
                    DisplayHeaderFooter == other.DisplayHeaderFooter &&
                    Equals(HeaderFile, other.HeaderFile) && // For byte array comparison
                    Equals(FooterFile, other.FooterFile) && // For byte array comparison
                    Landscape == other.Landscape &&
                    PaperFormat == other.PaperFormat &&
                    Background == other.Background &&
                    PageRange == other.PageRange &&
                    Scale == other.Scale &&
                    MarginTop == other.MarginTop &&
                    MarginRight == other.MarginRight &&
                    MarginBottom == other.MarginBottom &&
                    MarginLeft == other.MarginLeft &&
                    PageNumberFooterEnabled == other.PageNumberFooterEnabled &&
                    TitleHeaderEnabled == other.TitleHeaderEnabled &&
                    DateHeaderEnabled == other.DateHeaderEnabled &&
                    X == other.X &&
                    Y == other.Y &&
                    W == other.W &&
                    H == other.H &&
                    Width == other.Width &&
                    Height == other.Height &&
                    KeepRatio == other.KeepRatio &&
                    TextEnabled == other.TextEnabled &&
                    ImageMode == other.ImageMode &&
                    Compress == other.Compress &&
                    TransparencyEnabled == other.TransparencyEnabled &&
                    Equals(BackgroundColor, other.BackgroundColor);
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (TemplatingEngine?.GetHashCode() ?? 0);
                hash = hash * 23 + (RenderingEngine?.GetHashCode() ?? 0);
                hash = hash * 23 + DisplayHeaderFooter.GetHashCode();
                hash = hash * 23 + (HeaderFile?.GetHashCode() ?? 0);
                hash = hash * 23 + (FooterFile?.GetHashCode() ?? 0);
                hash = hash * 23 + Landscape.GetHashCode();
                hash = hash * 23 + (PaperFormat?.GetHashCode() ?? 0);
                hash = hash * 23 + Background.GetHashCode();
                hash = hash * 23 + (PageRange?.GetHashCode() ?? 0);
                hash = hash * 23 + Scale.GetHashCode();
                hash = hash * 23 + MarginTop.GetHashCode();
                hash = hash * 23 + MarginRight.GetHashCode();
                hash = hash * 23 + MarginBottom.GetHashCode();
                hash = hash * 23 + MarginLeft.GetHashCode();
                hash = hash * 23 + PageNumberFooterEnabled.GetHashCode();
                hash = hash * 23 + TitleHeaderEnabled.GetHashCode();
                hash = hash * 23 + DateHeaderEnabled.GetHashCode();
                hash = hash * 23 + (X?.GetHashCode() ?? 0);
                hash = hash * 23 + (Y?.GetHashCode() ?? 0);
                hash = hash * 23 + (W?.GetHashCode() ?? 0);
                hash = hash * 23 + (H?.GetHashCode() ?? 0);
                hash = hash * 23 + (Width?.GetHashCode() ?? 0);
                hash = hash * 23 + (Height?.GetHashCode() ?? 0);
                hash = hash * 23 + KeepRatio.GetHashCode();
                hash = hash * 23 + TextEnabled.GetHashCode();
                hash = hash * 23 + (ImageMode?.GetHashCode() ?? 0);
                hash = hash * 23 + Compress.GetHashCode();
                hash = hash * 23 + TransparencyEnabled.GetHashCode();
                hash = hash * 23 + (BackgroundColor?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var headerFile = HeaderFile != null 
                ? BitConverter.ToString(HeaderFile).Substring(0, Math.Min(20, HeaderFile.Length)) + "..." 
                : "null";
            var footerFile = FooterFile != null 
                ? BitConverter.ToString(FooterFile).Substring(0, Math.Min(20, FooterFile.Length)) + "..." 
                : "null";
            var backgroundColor = BackgroundColor != null ? string.Join(", ", BackgroundColor) : "null";
            
            return $"RenderOptions {{ TemplatingEngine = {TemplatingEngine}, RenderingEngine = {RenderingEngine}, " +
                $"DisplayHeaderFooter = {DisplayHeaderFooter}, HeaderFile = [{headerFile}], " +
                $"FooterFile = [{footerFile}], Landscape = {Landscape}, PaperFormat = {PaperFormat}, " +
                $"Background = {Background}, PageRange = {PageRange}, Scale = {Scale}, MarginTop = {MarginTop}, " +
                $"MarginRight = {MarginRight}, MarginBottom = {MarginBottom}, MarginLeft = {MarginLeft}, " +
                $"PageNumberFooterEnabled = {PageNumberFooterEnabled}, TitleHeaderEnabled = {TitleHeaderEnabled}, " +
                $"DateHeaderEnabled = {DateHeaderEnabled}, X = {X}, Y = {Y}, W = {W}, H = {H}, Width = {Width}, " +
                $"Height = {Height}, KeepRatio = {KeepRatio}, TextEnabled = {TextEnabled}, ImageMode = {ImageMode}, " +
                $"Compress = {Compress}, TransparencyEnabled = {TransparencyEnabled}, " +
                $"BackgroundColor = [{backgroundColor}] }}";
        }
    }
}