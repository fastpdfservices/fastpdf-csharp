// -----------------------------------------------------------------
// IPDFService.cs
// Created on 19 Nov 2023 by K.Edeline
// -----------------------------------------------------------------
//
// Description:
// The IPDFService.cs file contains the IPDFService interface, 
// which defines a set of operations for managing and manipulating 
// PDF documents in the FastPDFService library. This interface 
// abstracts the core functionalities like rendering, splitting, 
// merging, and compressing PDFs, providing a standard contract 
// for implementations.
//
// Implementations of this interface are responsible for carrying 
// out the actual PDF processing tasks, typically by interacting 
// with a remote PDF service or a local PDF generation tool.
//
// Usage:
// The IPDFService interface is designed to be implemented by 
// service classes that provide concrete PDF processing functionalities. 
// It allows for dependency injection and easier unit testing in 
// consuming applications.
//
// Example:
// public class PDFClient : IPDFService
// {
//     // Implementations of IPDFService methods...
// }
//
// -------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Threading.Tasks;
using FastPDFService.Models;

namespace FastPDFService
{
    /// <summary>
    /// IPDFService provides an interface for FastPDFService 
    /// </summary>
    public interface IPDFService
    {
        /// <summary>
        /// Saves the provided content to the specified file path.
        /// </summary>
        /// <param name="content">The byte array content to save.</param>
        /// <param name="filePath">The file path where the content should be saved.</param>
        public void Save(byte[] content, string filePath);

        /// <summary>
        /// Asynchronously saves the provided byte array content to the specified file path.
        /// </summary>
        /// <param name="content">The byte array content to save.</param>
        /// <param name="filePath">The file path where the content should be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task SaveAsync(byte[] content, string filePath);

        /// <summary>
        /// Extracts the files from the given zip file content.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <returns>A list of byte arrays representing the files extracted from the zip.</returns>
        public List<byte[]> Extract(byte[] zipBytes);

        /// <summary>
        /// Asynchronously extracts the files from the given zip file content.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <returns>A Task representing the asynchronous operation, containing a list of byte arrays for the extracted
        /// files.</returns>
        public Task<List<byte[]>> ExtractAsync(byte[] zipBytes);

        /// <summary>
        /// Extracts the files from the given zip file content and saves them to the specified output directory.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <param name="outputPath">The path of the output directory where the files should be saved.</param>
        public void ExtractToDirectory(byte[] zipBytes, string outputPath);

        /// <summary>
        /// Asynchronously extracts the files from the given zip file content and saves them to the specified output directory.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <param name="outputPath">The path of the output directory where the files should be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ExtractToDirectoryAsync(byte[] zipBytes, string outputPath);

        /* API CALLS */

        /// <summary>
        /// Validates the API token.
        /// </summary>
        /// <returns>true if the token is valid, false otherwise.</returns>
        public Task<bool> ValidateTokenAsync();

        /// <summary>
        /// Splits the PDF at the specified file path at the pages specified in the splits list.
        /// </summary>
        /// <param name="filePath">The path of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A byte array representing the split PDF.</returns>
        public Task<byte[]> SplitFromFileAsync(string filePath, List<int> splits);

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the split PDF.</returns>
        public Task<byte[]> SplitAsync(string fileContent, List<int> splits);

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the split PDF.</returns>
        public Task<byte[]> SplitAsync(byte[] fileContent, List<int> splits);

        /// <summary>
        /// Asynchronously edits the metadata of a PDF file at the specified file path with the provided metadata.
        /// </summary>
        /// <param name="filePath">The path of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public Task<byte[]> EditMetadataFromFileAsync(string filePath, Dictionary<string, string> metadata);

        /// <summary>
        /// Asynchronously edits the metadata of a PDF represented by the given byte array with the provided metadata.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public Task<byte[]> EditMetadataAsync(string fileContent, Dictionary<string, string> metadata);

        /// <summary>
        /// Asynchronously edits the metadata of a PDF represented by the given byte array with the provided metadata.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public Task<byte[]> EditMetadataAsync(byte[] fileContent, Dictionary<string, string> metadata);

        /// <summary>
        /// Asynchronously merges multiple PDFs whose paths are specified in the given list into a single PDF.
        /// </summary>
        /// <param name="filePaths">A list of file paths for the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public Task<byte[]> MergeFromFilePathsAsync(List<string> filePaths);

        /// <summary>
        /// Asynchronously merges multiple PDFs represented by the given list of byte arrays into a single PDF.
        /// </summary>
        /// <param name="fileContents">A list of byte arrays representing the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public Task<byte[]> MergeFromStringsAsync(List<string> fileContents);

        /// <summary>
        /// Asynchronously merges multiple PDFs represented by the given list of byte arrays into a single PDF.
        /// </summary>
        /// <param name="fileContents">A list of byte arrays representing the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public Task<byte[]> MergeFromBytesAsync(List<byte[]> fileContents);

        /// <summary>
        /// Asynchronously splits a PDF at the specified file path at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="filePath">The path of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>
        public Task<byte[]> SplitZipFromFileAsync(string filePath, List<List<int>> splits);

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>;
        public Task<byte[]> SplitZipAsync(string fileContent, List<List<int>> splits);

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>
        public Task<byte[]> SplitZipAsync(byte[] fileContent, List<List<int>> splits);

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public Task<byte[]> ToImageFromFileAsync(string filePath, string outputFormat);

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        ///  <param name="fileContent">The PDF file to convert.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public Task<byte[]> ToImageAsync(string fileContent, string outputFormat);

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        ///  <param name="fileContent">The PDF file to convert.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public Task<byte[]> ToImageAsync(byte[] fileContent, string outputFormat);

        /// <summary>
        /// Asynchronously compresses a PDF file with optional compression options.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="options">Optional compression options (e.g., remove duplicate images).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the compressed file.</returns>
        public Task<byte[]> CompressFromFileAsync(string filePath, Dictionary<string, bool>? options = null);

        /// <summary>
        /// Asynchronously compresses a PDF file with optional compression options.
        /// </summary>
        ///  <param name="fileContent">The PDF file to compress.</param>
        /// <param name="options">Optional compression options (e.g., remove duplicate images).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the compressed file.</returns>
        public Task<byte[]> CompressAsync(string fileContent, Dictionary<string, bool>? options = null);

        /// <summary>
        /// Asynchronously compresses a PDF file with optional compression options.
        /// </summary>
        ///  <param name="fileContent">The PDF file to compress.</param>
        /// <param name="options">Optional compression options (e.g., remove duplicate images).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the compressed file.</returns>
        public Task<byte[]> CompressAsync(byte[] fileContent, Dictionary<string, bool>? options = null);

        /// <summary>
        /// Asynchronously converts a webpage URL to a PDF.
        /// </summary>
        /// <param name="url">The URL of the webpage to convert.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted PDF.</returns>
        public Task<byte[]> UrlToPdfAsync(string url);

        /// <summary>
        /// Asynchronously renders a barcode with specified data, format, and rendering options.
        /// </summary>
        /// <param name="data">The data to encode in the barcode.</param>
        /// <param name="barcodeFormat">The format of the barcode to render (default is "code128").</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the barcode image.</returns>
        public Task<byte[]> RenderBarcodeAsync(
            string data, 
            string barcodeFormat = "code128", 
            RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public Task<byte[]> RenderImageFromFileAsync(string filePath, RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="fileContent">The image file to render.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public Task<byte[]> RenderImageAsync(string fileContent, RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="fileContent">The image file to render.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public Task<byte[]> RenderImageAsync(byte[] fileContent, RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders an image from a given image ID with rendering options.
        /// </summary>
        /// <param name="imageId">The ID of the image to be rendered.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public Task<byte[]> RenderImageFromIdAsync(string imageId, RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously retrieves all available templates with an optional limit.
        /// </summary>
        /// <param name="limit">Optional maximum number of templates to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, containing a list of available templates.</returns>
        public Task<List<Template>?> GetAllTemplatesAsync(int? limit = null);

        /// <summary>
        /// Asynchronously retrieves a template by its ID.
        /// </summary>
        /// <param name="templateId">The ID of the template to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, containing the template associated with the given ID.</returns>
        public Task<Template?> GetTemplateAsync(string templateId);

        /// <summary>
        /// Asynchronously adds a new template to the system.
        /// </summary>
        /// <param name="filePath">The path to the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFilePath">Optional path to the file containing the header HTML code.</param>
        /// <param name="footerFilePath">Optional path to the file containing the footer HTML code.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public Task<Template?> AddTemplateFromFileAsync(
            string filePath, 
            Template templateData, 
            string? headerFilePath = null, 
            string? footerFilePath = null);

        /// <summary>
        /// Asynchronously adds a new template to the system with file content, header, and footer.
        /// </summary>
        /// <param name="fileContent">The content of the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFileContent">Optional content of the HTML header.</param>
        /// <param name="footerFileContent">Optional content of the HTML footer.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public Task<Template?> AddTemplateAsync(
            byte[] fileContent, 
            Template templateData, 
            byte[]? headerFileContent = null, 
            byte[]? footerFileContent = null);

        /// <summary>
        /// Asynchronously adds a new template to the system with file content, header, and footer.
        /// </summary>
        /// <param name="fileContent">The content of the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFileContent">Optional content of the HTML header.</param>
        /// <param name="footerFileContent">Optional content of the HTML footer.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public Task<Template?> AddTemplateAsync(
            string fileContent, 
            Template templateData, 
            string? headerFileContent = null, 
            string? footerFileContent = null);

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="filePath">The path of the stylesheet file on the local filesystem.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly
        /// added stylesheet.</returns>
        public Task<StyleFile?> AddStylesheetFromFileAsync(string templateId, string filePath, StyleFile styleFileData);

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="fileContent">The content of the file for the new stylesheet.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly added
        /// stylesheet.</returns>
        public Task<StyleFile?> AddStylesheetAsync(string templateId, byte[] fileContent, StyleFile styleFileData);

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="fileContent">The content of the file for the new stylesheet.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly added
        /// stylesheet.</returns>
        public Task<StyleFile?> AddStylesheetAsync(string templateId, string fileContent, StyleFile styleFileData);

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="filePath">The path of the image file on the local filesystem.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public Task<ImageFile?> AddImageFromFileAsync(string templateId, string filePath, ImageFile imageFileData);

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="fileContent">The content of the file for the new image.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public Task<ImageFile?> AddImageAsync(string templateId, byte[] fileContent, ImageFile imageFileData);

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="fileContent">The content of the file for the new image.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public Task<ImageFile?> AddImageAsync(string templateId, string fileContent, ImageFile imageFileData);

        /// <summary>
        /// Asynchronously deletes a template.
        /// </summary>
        /// <param name="templateId">The ID of the template to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the template was successfully
        /// deleted.</returns>
        public Task<bool> DeleteTemplateAsync(string templateId);

        /// <summary>
        /// Asynchronously deletes a stylesheet.
        /// </summary>
        /// <param name="stylesheetId">The ID of the stylesheet to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the stylesheet was successfully
        /// deleted.</returns>
        public Task<bool> DeleteStylesheetAsync(string stylesheetId);

        /// <summary>
        /// Asynchronously deletes a image.
        /// </summary>
        /// <param name="imageId">The ID of the image to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the image was successfully
        /// deleted.</returns>
        public Task<bool> DeleteImageAsync(string imageId);

        /// <summary>
        /// Asynchronously retrieves the file of a template.
        /// </summary>
        /// <param name="templateId">The ID of the template whose file is to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the template file as a
        /// byte array.</returns>
        public Task<byte[]> GetTemplateFileAsync(string templateId);

        /// <summary>
        /// Asynchronously retrieves a stylesheet.
        /// </summary>
        /// <param name="stylesheetId">The ID of the stylesheet to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the stylesheet as a byte
        /// array.</returns>
        public Task<byte[]> GetStylesheetAsync(string stylesheetId);

        /// <summary>
        /// Asynchronously retrieves an image.
        /// </summary>
        /// <param name="imageId">The ID of the image to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the image as a byte
        /// array.</returns>
        public Task<byte[]> GetImageAsync(string imageId);

        /// <summary>
        /// Asynchronously renders a template with the provided render data from a file path and render options.
        /// Output format is set to PDF by default.
        /// </summary>
        /// <param name="templateId">The ID of the template to render.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="renderOptions">The rendering options to use.</param>
        /// <param name="formatType">The output format type  (Optional, default to pdf).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered
        /// template.</returns>
        public Task<byte[]> RenderTemplateAsync(
            string templateId, 
            string renderDataPath, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders a template with the provided render data from a file path and render options.
        /// Output format is set to PDF by default.
        /// </summary>
        /// <param name="templateId">The ID of the template to render.</param>
        /// <param name="renderData">The render data to use.</param>
        /// <param name="renderOptions">The rendering options to use.</param>
        /// <param name="formatType">The output format type  (Optional, default to pdf).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered template.</returns>
        public Task<byte[]> RenderTemplateAsync(
            string templateId, 
            Dictionary<string, object>? renderData, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template with the provided render data and render options,
        /// and a specified format type.
        /// </summary>
        /// <param name="templateId">The ID of the template to render.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="renderOptions">The rendering options to use (Optional).</param>
        /// <param name="formatType">The output format type  (Optional, default to pdf).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered
        /// templates.</returns>
        /// <throws>ArgumentException if the render data is not a list of dictionaries.</throws>
        public Task<byte[]> RenderTemplateManyAsync(
            string templateId, 
            string renderDataPath, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template with the provided render data and render options,
        /// and a specified format type.
        /// </summary>
        /// <param name="templateId">The ID of the template to render.</param>
        /// <param name="renderDataList">The list of render data to use.</param>
        /// <param name="renderOptions">The rendering options to use (Optional).</param>
        /// <param name="formatType">The output format type (Optional, default to pdf).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered
        /// templates.</returns>
        public Task<byte[]> RenderTemplateManyAsync(
            string templateId, 
            List<Dictionary<string, object>>? renderDataList, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders a single template with given parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data or null.</param>
        /// <param name="renderData">The render data.</param>
        /// <param name="headerFilePath">The path of the header file or null.</param>
        /// <param name="footerFilePath">The path of the footer file or null.</param>
        /// <param name="renderOptions">The options for rendering the template or null.</param>
        /// <param name="formatType">The type of the output format.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered template as a byte
        /// array.</returns>
        public Task<byte[]> RenderFromFileAsync(
            string templateFilePath,
            Template? template = null,
            object? renderData = null,
            string? headerFilePath = null,
            string? footerFilePath = null,
            RenderOptions? renderOptions = null,
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders a template from its content and provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered template as a byte array.</returns>
        public Task<byte[]> RenderAsync(
            byte[] templateFileContent,
            Template? template = null,
            Dictionary<string, object>? renderData = null,
            byte[]? headerFileContent = null,
            byte[]? footerFileContent = null,
            RenderOptions? renderOptions = null,
            string formatType = "pdf");

                /// <summary>
        /// Asynchronously renders a template from its content and provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered template as a byte array.</returns>
        public Task<byte[]> RenderAsync(
            string templateFileContent,
            Template? template = null,
            Dictionary<string, object>? renderData = null,
            string? headerFileContent = null,
            string? footerFileContent = null,
            RenderOptions? renderOptions = null,
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders a PDF from a template file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="headerFilePath">The path of the header file, if any.</param>
        /// <param name="footerFilePath">The path of the footer file, if any.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PDF as a byte array.
        /// </returns>
        public Task<byte[]> RenderFromFileToPDFAsync(
            string templateFilePath,
            string? renderDataPath = null,
            string? headerFilePath = null,
            string? footerFilePath = null,
            Template? template = null,
            RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders a PDF from a template file path with provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PDF as a byte array.
        /// </returns>
        public Task<byte[]> RenderToPDFAsync(
            byte[] templateFileContent,
            Dictionary<string, object>? renderData = null,
            byte[]? headerFileContent = null,
            byte[]? footerFileContent = null,
            Template? template = null,
            RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders a PDF from a template file path with provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PDF as a byte array.
        /// </returns>
        public Task<byte[]> RenderToPDFAsync(
            string templateFileContent,
            Dictionary<string, object>? renderData = null,
            string? headerFileContent = null,
            string? footerFileContent = null,
            Template? template = null,
            RenderOptions? renderOptions = null);

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a
        /// byte array.</returns>
        public Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template? template = null,
            string? renderDataPath = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="headerFilePath">The path of the header file, if any.</param>
        /// <param name="footerFilePath">The path of the footer file, if any.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a byte
        /// array.</returns>
        public Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template? template = null,
            string? renderDataPath = null, 
            string? headerFilePath = null, 
            string? footerFilePath = null,
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="headerFilePath">The path of the header file, if any.</param>
        /// <param name="footerFilePath">The path of the footer file, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a byte array.</returns>
        public Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template? template = null,
            string? renderDataPath = null, 
            string? headerFilePath = null, 
            string? footerFilePath = null,
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its content with provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataList">The list of render data to be used.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a byte array.
        /// </returns>
        public Task<byte[]> RenderManyAsync(
            byte[] templateFileContent, 
            Template? template = null,
            List<Dictionary<string, object>>? renderDataList = null, 
            byte[]? headerFileContent = null,
            byte[]? footerFileContent = null, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its content with provided parameters.
        /// </summary>
        /// <param name="templateFileContent">The content of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataList">The list of render data to be used.</param>
        /// <param name="headerFileContent">The content of the header file, if any.</param>
        /// <param name="footerFileContent">The content of the footer file, if any.</param>
        /// <param name="renderOptions">The options for rendering the template.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a byte array.
        /// </returns>
        public Task<byte[]> RenderManyAsync(
            string templateFileContent, 
            Template? template = null,
            List<Dictionary<string, object>>? renderDataList = null, 
            string? headerFileContent = null,
            string? footerFileContent = null, 
            RenderOptions? renderOptions = null, 
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataList">The list of render data to be used.</param>
        /// <param name="headerFilePath">The path of the header file, if any.</param>
        /// <param name="footerFilePath">The path of the footer file, if any.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a byte array.
        /// </returns>
        public Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template? template = null,
            List<Dictionary<string, object>>? renderDataList = null, 
            string? headerFilePath = null, 
            string? footerFilePath = null,
            string formatType = "pdf");

        /// <summary>
        /// Asynchronously renders a template to a PDF format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PDF as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToPdfAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a DOCX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered DOCX as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToDocxAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a ODP format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODP as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToOdpAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a ODS format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODS as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToOdsAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a ODT format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODT as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToOdtAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a PPTX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PPTX as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToPptxAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a XLX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered XLX as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToXlxAsync(string templateId, Dictionary<string, object>? renderData);

        /// <summary>
        /// Asynchronously renders a template to a XLS format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered XLS as a byte array.
        /// </returns>
        public Task<byte[]> RenderTemplateToXlsAsync(string templateId, Dictionary<string, object>? renderData);
    }
}