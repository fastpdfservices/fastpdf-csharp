using System.Collections.Generic;
using System.Dynamic;

namespace FastPDFService.Models;

/// <summary>
/// Represents settings for the FastPDFService client.
/// </summary>
public class PdfClientSettings
{
    /// <summary>
    /// The base URL for the PDF service.
    /// </summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// The root base URL from which the base URL will be built.
    /// </summary>
    public string RootBaseUrl { get; set; } = "https://data.fastpdfservice.com";
    
    /// <summary>
    /// The version of the API to use.
    /// </summary>
    public string ApiVersion { get; set; } = "v1";
    
    /// <summary>
    /// A list of supported image formats. Supported formats include
    /// jpeg, png, gif, bmp, tiff, webp, svg, ico, pdf, psd, ai, eps,
    /// cr2, nef, sr2, orf, rw2, dng, arw, and heic.
    /// </summary>
    public List<string> SupportedImageFormats { get; set; } = new()
    {
        "jpeg", "png", "gif", "bmp", "tiff", "webp", "svg", "ico", "pdf",
        "psd", "ai", "eps", "cr2", "nef", "sr2", "orf", "rw2", "dng",
        "arw", "heic"
    };

    /// <summary>
    /// A list of supported barcode formats. Supported formats include
    /// codabar, code128, code39, ean, ean13, ean13-guard, ean14, ean8,
    /// ean8-guard, gs1, gs1_128, gtin, isbn, isbn10, isbn13, issn, itf, jan,
    /// nw-7, pzn, upc, upca.
    /// </summary>
    public List<string> SupportedBarcodeFormats { get; set; } = new()
    {
        "codabar", "code128", "code39", "ean", "ean13", "ean13-guard",
        "ean14", "ean8", "ean8-guard", "gs1", "gs1_128", "gtin", "isbn",
        "isbn10", "isbn13", "issn", "itf", "jan", "nw-7", "pzn", "upc",
        "upca"
    };

    /// <summary>
    /// Generates the base URL for the PDF service.
    /// </summary>
    public void BuildBaseUrl()
    {
        BaseUrl = RootBaseUrl.EndsWith("/") ? RootBaseUrl + ApiVersion : RootBaseUrl + "/" + ApiVersion;
    }
}