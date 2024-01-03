// -----------------------------------------------------------------
// PDFClient.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//  The PDFClient.cs file is part of the FastPDFService library, a C# client 
//  for interacting with the FastPDFService API. This file contains the PDFClient 
//  class, which provides methods for various PDF operations like rendering, splitting, 
//  merging, compressing, and converting web content or images to PDF. It also includes 
//  functionality for working with templates, stylesheets, and images in the context 
//  of PDF generation.
// -----------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MimeDetective;

using FastPDFService.Exceptions;
using FastPDFService.Models;

namespace FastPDFService
{
    /// <summary>
    /// The PDFClient class designed to facilitate interactions with the FastPDFService API.
    /// It encapsulates functionalities for creating, managing, and manipulating PDF documents.
    /// This includes rendering PDFs from templates or URLs, adding images and stylesheets to templates, 
    /// and performing various operations on existing PDF files like splitting, merging, and compressing.
    /// 
    /// For more information, visit: https://docs.fastpdfservice.com
    /// </summary>
    public class PDFClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ContentInspector _inspector;
        private readonly PdfClientSettings _settings = new PdfClientSettings();

        /// <summary>
        /// The encoding of input strings (default: Unicode/UTF16)
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Unicode;

        /// <summary>
        /// Constructs a PDFClient with the provided API key, base URL, and API version.
        /// </summary>
        /// <param name="apiKey">The API key to authenticate requests.</param>
        /// <param name="baseUrl">The base URL for the PDF service.</param>
        /// <param name="apiVersion">The version of the API to use.</param>
        public PDFClient(string apiKey, string baseUrl = null, string apiVersion = null)
        {
            _apiKey = apiKey;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _settings.RootBaseUrl = baseUrl;
            }
            if (!string.IsNullOrEmpty(apiVersion))
            {
                _settings.ApiVersion = apiVersion;
            }
            _settings.BuildBaseUrl();
            _httpClient = new HttpClient();
            _inspector =  new ContentInspectorBuilder
            {
                Definitions = MimeDetective.Definitions.Default.All()
            }.Build();

            // Configure JSON serializer settings if needed
            ConfigureJsonSerializer();
        }

        /// <summary>
        /// Saves the provided content to the specified file path.
        /// </summary>
        /// <param name="content">The byte array content to save.</param>
        /// <param name="filePath">The file path where the content should be saved.</param>
        public void Save(byte[] content, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                File.WriteAllBytes(filePath, content);
            }
        }

        /// <summary>
        /// Asynchronously saves the provided byte array content to the specified file path.
        /// </summary>
        /// <param name="content">The byte array content to save.</param>
        /// <param name="filePath">The file path where the content should be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task SaveAsync(byte[] content, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                await File.WriteAllBytesAsync(filePath, content);
            }
        }

        /// <summary>
        /// Extracts the files from the given zip file content.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <returns>A list of byte arrays representing the files extracted from the zip.</returns>
        public List<byte[]> Extract(byte[] zipBytes)
        {
            var files = new List<byte[]>();
            using var zipStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(zipStream);
            foreach (var entry in archive.Entries)
            {
                using var entryStream = entry.Open();
                using var outputStream = new MemoryStream();
                entryStream.CopyTo(outputStream);
                files.Add(outputStream.ToArray());
            }

            return files;
        }

        /// <summary>
        /// Asynchronously extracts the files from the given zip file content.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <returns>A Task representing the asynchronous operation, containing a list of byte arrays for the extracted
        /// files.</returns>
        public async Task<List<byte[]>> ExtractAsync(byte[] zipBytes)
        {
            var files = new List<byte[]>();
            using var zipStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(zipStream);
            foreach (var entry in archive.Entries)
            {
                await using var entryStream = entry.Open();
                using var outputStream = new MemoryStream();
                await entryStream.CopyToAsync(outputStream);
                files.Add(outputStream.ToArray());
            }

            return files;
        }

        /// <summary>
        /// Extracts the files from the given zip file content and saves them to the specified output directory.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <param name="outputPath">The path of the output directory where the files should be saved.</param>
        public void ExtractToDirectory(byte[] zipBytes, string outputPath)
        {
            using var zipStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(zipStream);
            foreach (var entry in archive.Entries)
            {
                var completeFilePath = Path.Combine(outputPath, entry.FullName);
                var directoryPath = Path.GetDirectoryName(completeFilePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath!);
                }

                using var fileStream = new FileStream(completeFilePath, FileMode.Create);
                using var entryStream = entry.Open();
                entryStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Asynchronously extracts the files from the given zip file content and saves them to the specified output directory.
        /// </summary>
        /// <param name="zipBytes">The content of the zip file.</param>
        /// <param name="outputPath">The path of the output directory where the files should be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task ExtractToDirectoryAsync(byte[] zipBytes, string outputPath)
        {
            using var zipStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(zipStream);
            foreach (var entry in archive.Entries)
            {
                var completeFilePath = Path.Combine(outputPath, entry.FullName);
                var directoryPath = Path.GetDirectoryName(completeFilePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath!);
                }

                await using var fileStream = new FileStream(completeFilePath, FileMode.Create);
                await using var entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream);
            }
        }

        /* API CALLS */

        /// <summary>
        /// Validates the API token.
        /// </summary>
        /// <returns>true if the token is valid, false otherwise.</returns>
        public async Task<bool> ValidateTokenAsync()
        {
            var response = await GetAsync($"{_settings.BaseUrl}/token");
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Splits the PDF at the specified file path at the pages specified in the splits list.
        /// </summary>
        /// <param name="filePath">The path of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A byte array representing the split PDF.</returns>
        public async Task<byte[]> SplitFromFileAsync(string filePath, List<int> splits)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await SplitAsync(fileContent, splits);
        }

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the split PDF.</returns>
        public async Task<byte[]> SplitAsync(string fileContent, List<int> splits)
        {
           return await SplitAsync(Encoding.GetBytes(fileContent), splits);
        }

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of integers representing the pages where the PDF should be split.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the split PDF.</returns>
        public async Task<byte[]> SplitAsync(byte[] fileContent, List<int> splits)
        {
            using var content = new MultipartFormDataContent
            {
                { GetPDFByteArray(fileContent), "file", "file.pdf" },
                { new StringContent(JsonConvert.SerializeObject(splits)), "splits" }
            };
            return await PostAsync($"{_settings.BaseUrl}/pdf/split", content);
        }

        /// <summary>
        /// Asynchronously edits the metadata of a PDF file at the specified file path with the provided metadata.
        /// </summary>
        /// <param name="filePath">The path of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public async Task<byte[]> EditMetadataFromFileAsync(string filePath, Dictionary<string, string> metadata)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await EditMetadataAsync(fileContent, metadata);
        }

        /// <summary>
        /// Asynchronously edits the metadata of a PDF represented by the given byte array with the provided metadata.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public async Task<byte[]> EditMetadataAsync(string fileContent, Dictionary<string, string> metadata)
        {
            return await EditMetadataAsync(Encoding.GetBytes(fileContent), metadata);
        }

        /// <summary>
        /// Asynchronously edits the metadata of a PDF represented by the given byte array with the provided metadata.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file whose metadata is to be edited.</param>
        /// <param name="metadata">A dictionary containing the new metadata for the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the PDF with updated metadata.</returns>
        public async Task<byte[]> EditMetadataAsync(byte[] fileContent, Dictionary<string, string> metadata)
        {
            using var content = new MultipartFormDataContent
            {
                { GetPDFByteArray(fileContent), "file", "file.pdf" },
                { new StringContent(JsonConvert.SerializeObject(metadata)), "metadata" }
            };
            return await PostAsync($"{_settings.BaseUrl}/pdf/metadata", content);
        }

        /// <summary>
        /// Asynchronously merges multiple PDFs whose paths are specified in the given list into a single PDF.
        /// </summary>
        /// <param name="filePaths">A list of file paths for the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public async Task<byte[]> MergeFromFilePathsAsync(List<string> filePaths)
        {
            if (filePaths.Count < 2 || filePaths.Count > 100)
            {
                throw new ArgumentException("The number of files to merge should be between 2 and 100.");
            }

            using var content = new MultipartFormDataContent();
            for (var i = 0; i < filePaths.Count; i++)
            {
                var fileBytes = await File.ReadAllBytesAsync(filePaths[i]);
                content.Add(GetPDFByteArray(fileBytes), $"file{i}", "file.pdf");
            }

            return await MergeAsync(content);
        }

        /// <summary>
        /// Asynchronously merges multiple PDFs represented by the given list of byte arrays into a single PDF.
        /// </summary>
        /// <param name="fileContents">A list of byte arrays representing the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public async Task<byte[]> MergeFromStringsAsync(List<string> fileContents)
        {
            List<byte[]> fileContentsByte = new List<byte[]>();
            foreach (var fileContent in fileContents)
            {
                var fileBytes = Encoding.GetBytes(fileContent);
                fileContentsByte.Add(fileBytes);
            }

            return await MergeFromBytesAsync(fileContentsByte);
        }

        /// <summary>
        /// Asynchronously merges multiple PDFs represented by the given list of byte arrays into a single PDF.
        /// </summary>
        /// <param name="fileContents">A list of byte arrays representing the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        public async Task<byte[]> MergeFromBytesAsync(List<byte[]> fileContents)
        {
            if (fileContents.Count < 2 || fileContents.Count > 100)
            {
                throw new ArgumentException("The number of files to merge should be between 2 and 100.");
            }

            using var content = new MultipartFormDataContent();
            for (var i = 0; i < fileContents.Count; i++)
            {
                content.Add(GetPDFByteArray(fileContents[i]), $"file{i}", "file.pdf");
            }

            return await MergeAsync(content);
        }

        /// <summary>
        /// Asynchronously splits a PDF at the specified file path at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="filePath">The path of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>
        public async Task<byte[]> SplitZipFromFileAsync(string filePath, List<List<int>> splits)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await SplitZipAsync(fileContent, splits);
        }

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>
        public async Task<byte[]> SplitZipAsync(string fileContent, List<List<int>> splits)
        {
            return await SplitZipAsync(Encoding.GetBytes(fileContent), splits);
        }

        /// <summary>
        /// Asynchronously splits a PDF represented by the given byte array at the pages specified in the splits list
        /// and returns a zip file containing the split PDFs.
        /// </summary>
        /// <param name="fileContent">The content of the PDF file to be split.</param>
        /// <param name="splits">A list of lists of integers where each sublist represents the pages for a single split of the PDF.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the zip file of split PDFs.</returns>
        public async Task<byte[]> SplitZipAsync(byte[] fileContent, List<List<int>> splits)
        {
            using var content = new MultipartFormDataContent
            {
                { GetPDFByteArray(fileContent), "file", "file.pdf" },
                { new StringContent(JsonConvert.SerializeObject(splits)), "splits" }
            };

            return await PostAsync($"{_settings.BaseUrl}/pdf/split-zip", content);
        }

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public async Task<byte[]> ToImageFromFileAsync(string filePath, string outputFormat)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await ToImageAsync(fileContent, outputFormat);
        }

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        ///  <param name="fileContent">The PDF file to convert.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public async Task<byte[]> ToImageAsync(string fileContent, string outputFormat)
        {
            return await ToImageAsync(Encoding.GetBytes(fileContent), outputFormat);
        }

        /// <summary>
        /// Asynchronously converts a PDF file to an image in the specified format.
        /// </summary>
        ///  <param name="fileContent">The PDF file to convert.</param>
        /// <param name="outputFormat">The desired image format (e.g., "jpeg", "png").</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted image.</returns>
        public async Task<byte[]> ToImageAsync(byte[] fileContent, string outputFormat)
        {
            outputFormat = outputFormat.ToLowerInvariant();
            if (!_settings.SupportedImageFormats.Contains(outputFormat))
            {
                throw new ArgumentException($"Unsupported output format. Must be one of: {string.Join(", ", _settings.SupportedImageFormats)}");
            }

            using var content = new MultipartFormDataContent
            {
                { GetPDFByteArray(fileContent), "file", "file.pdf" }
            };
            return await PostAsync($"{_settings.BaseUrl}/pdf/image/{outputFormat}", content);
        }

        /// <summary>
        /// Asynchronously compresses a PDF file with optional compression options.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="options">Optional compression options (e.g., remove duplicate images).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the compressed file.</returns>
        public async Task<byte[]> CompressFromFileAsync(string filePath, Dictionary<string, bool> options = null)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await CompressAsync(fileContent, options);
        }

        /// <summary>
        /// Asynchronously compresses a PDF file with optional compression options.
        /// </summary>
        ///  <param name="fileContent">The PDF file to compress.</param>
        /// <param name="options">Optional compression options (e.g., remove duplicate images).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the compressed file.</returns>
        public async Task<byte[]> CompressAsync(byte[] fileContent, Dictionary<string, bool> options = null)
        {
            using var content = new MultipartFormDataContent
            {
                { GetPDFByteArray(fileContent), "file", "file.pdf" }
            };

            if (options != null)
            {
                content.Add(new StringContent(JsonConvert.SerializeObject(options)), "options");
            }

            return await PostAsync($"{_settings.BaseUrl}/pdf/compress", content);
        }

        /// <summary>
        /// Asynchronously encrypts a PDF file with a password.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="password">The password for encryption.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the encrypted file.</returns>
        public async Task<byte[]> EncryptFromFileAsync(string filePath, string password)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await EncryptAsync(fileContent, password);
        }

        /// <summary>
        /// Asynchronously encrypts a PDF file with a password.
        /// </summary>
        /// <param name="fileContent">The PDF file to encrypt.</param>
        /// <param name="password">The password for encryption.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the encrypted file.</returns>
        public async Task<byte[]> EncryptAsync(byte[] fileContent, string password)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(GetPDFByteArray(fileContent), "file", "file.pdf");
                var passwordOptions = new Dictionary<string, string>
                {
                    { "encrypt_password", password }
                };
                content.Add(new StringContent(JsonConvert.SerializeObject(passwordOptions)), "options");

                return await PostAsync($"{_settings.BaseUrl}/pdf/encrypt", content);
            }
        }

        /// <summary>
        /// Asynchronously converts a webpage URL to a PDF.
        /// </summary>
        /// <param name="url">The URL of the webpage to convert.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the converted PDF.</returns>
        public async Task<byte[]> UrlToPdfAsync(string url)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(url), "url" }
            };

            return await PostAsync($"{_settings.BaseUrl}/pdf/url", content);
        }

        /// <summary>
        /// Asynchronously renders a barcode with specified data, format, and rendering options.
        /// </summary>
        /// <param name="data">The data to encode in the barcode.</param>
        /// <param name="barcodeFormat">The format of the barcode to render (default is "code128").</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the barcode image.</returns>
        public async Task<byte[]> RenderBarcodeAsync(
            string data, 
            string barcodeFormat = "code128", 
            RenderOptions renderOptions = null)
        {
            var dataMap = new Dictionary<string, object>
            {
                { "data", data },
                { "barcode_format", barcodeFormat.ToLowerInvariant() }
            };

            var requestMap = new Dictionary<string, object>
            {
                { "data", dataMap }
            };

            if (renderOptions != null)
            {
                requestMap.Add("render_options", renderOptions);
            }

            var jsonContent = JsonConvert.SerializeObject(requestMap);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await PostAsync($"{_settings.BaseUrl}/render/barcode", content);
        }

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public async Task<byte[]> RenderImageFromFileAsync(string filePath, RenderOptions renderOptions = null)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            return await RenderImageAsync(fileContent, renderOptions);
        }

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="fileContent">The image file to render.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public async Task<byte[]> RenderImageAsync(string fileContent, RenderOptions renderOptions = null)
        {
            return await RenderImageAsync(Encoding.GetBytes(fileContent), renderOptions);
        }

        /// <summary>
        /// Asynchronously renders an image from a file path with optional rendering options.
        /// </summary>
        /// <param name="fileContent">The image file to render.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public async Task<byte[]> RenderImageAsync(byte[] fileContent, RenderOptions renderOptions = null)
        {
            using var content = new MultipartFormDataContent();
            var fileByteArrayContent = new ByteArrayContent(fileContent);
            fileByteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(GetFileMimeType(fileContent));
            content.Add(fileByteArrayContent, "image", "image");

            if (renderOptions == null)
            {
                return await PostAsync($"{_settings.BaseUrl}/render/img", content);
            }
            
            var serializedObject = JsonConvert.SerializeObject(renderOptions);
            var stringContent = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            content.Add(stringContent, "render_options"); // Adjust serialization if needed

            return await PostAsync($"{_settings.BaseUrl}/render/img", content);
        }

        /// <summary>
        /// Asynchronously renders an image from a given image ID with rendering options.
        /// </summary>
        /// <param name="imageId">The ID of the image to be rendered.</param>
        /// <param name="renderOptions">Optional rendering options.</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered image.</returns>
        public async Task<byte[]> RenderImageFromIdAsync(string imageId, RenderOptions renderOptions = null)
        {
            using var content = new MultipartFormDataContent();
            if (renderOptions != null)
            {
                var serializedObject = JsonConvert.SerializeObject(renderOptions);
                var stringContent = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                content.Add(stringContent, "render_options"); // Adjust serialization if needed
            }
        
            return await PostAsync($"{_settings.BaseUrl}/img/{imageId}", content);
        }

        /// <summary>
        /// Asynchronously retrieves all available templates with an optional limit.
        /// </summary>
        /// <param name="limit">Optional maximum number of templates to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, containing a list of available templates.</returns>
        public async Task<List<Template>> GetAllTemplatesAsync(int? limit = null)
        {
            var url = $"{_settings.BaseUrl}/template";
            if (limit.HasValue)
            {
                url += $"?limit={limit.Value}";
            }

            return await GetAsync<List<Template>>(url);
        }

        /// <summary>
        /// Asynchronously retrieves a template by its ID.
        /// </summary>
        /// <param name="templateId">The ID of the template to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, containing the template associated with the given ID.</returns>
        public async Task<Template> GetTemplateAsync(string templateId)
        {
            return await GetAsync<Template>($"{_settings.BaseUrl}/template/{templateId}");
        }

        /// <summary>
        /// Asynchronously adds a new template to the system.
        /// </summary>
        /// <param name="filePath">The path to the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFilePath">Optional path to the file containing the header HTML code.</param>
        /// <param name="footerFilePath">Optional path to the file containing the footer HTML code.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public async Task<Template> AddTemplateFromFileAsync(
            string filePath, 
            Template templateData, 
            string headerFilePath = null, 
            string footerFilePath = null)
        {
            using var fileContent = new StreamContent(File.OpenRead(filePath));
            using var headerContent = headerFilePath != null ? new StreamContent(File.OpenRead(headerFilePath)) : null;
            using var footerContent = footerFilePath != null ? new StreamContent(File.OpenRead(footerFilePath)) : null;

            var filename = Path.GetFileName(filePath);
            var headerFilename = headerFilePath != null ? Path.GetFileName(headerFilePath) : null;
            var footerFilename = footerFilePath != null ? Path.GetFileName(footerFilePath) : null;

            return await AddTemplateAsync(
                fileContent, 
                templateData, 
                headerContent, 
                footerContent, 
                filename, 
                headerFilename, 
                footerFilename);
        }

        /// <summary>
        /// Asynchronously adds a new template to the system with file content, header, and footer.
        /// </summary>
        /// <param name="fileContent">The content of the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFileContent">Optional content of the HTML header.</param>
        /// <param name="footerFileContent">Optional content of the HTML footer.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public async Task<Template> AddTemplateAsync(
            byte[] fileContent, 
            Template templateData, 
            byte[] headerFileContent = null, 
            byte[] footerFileContent = null)
        {
            using var fileStreamContent = new ByteArrayContent(fileContent);
            using var headerStreamContent = headerFileContent != null ? new ByteArrayContent(headerFileContent) : null;
            using var footerStreamContent = footerFileContent != null ? new ByteArrayContent(footerFileContent) : null;

            var filename = "file." + templateData.Format;
            const string headerFilename = "header.html";
            const string footerFilename = "footer.html";

            return await AddTemplateAsync(
                fileStreamContent, 
                templateData, 
                headerStreamContent, 
                footerStreamContent, 
                filename, 
                headerFilename, 
                footerFilename);
        }

        /// <summary>
        /// Asynchronously adds a new template to the system with file content, header, and footer.
        /// </summary>
        /// <param name="fileContent">The content of the file for the new template.</param>
        /// <param name="templateData">The data for the new template.</param>
        /// <param name="headerFileContent">Optional content of the HTML header.</param>
        /// <param name="footerFileContent">Optional content of the HTML footer.</param>
        /// <returns>A Task representing the asynchronous operation, containing the newly created template.</returns>
        public async Task<Template> AddTemplateAsync(
            string fileContent, 
            Template templateData, 
            string headerFileContent = null, 
            string footerFileContent = null)
        {
            byte[] headerFileContentBytes = headerFileContent == null ? null : Encoding.GetBytes(headerFileContent);
            byte[] footerFileContentBytes = footerFileContent == null ? null : Encoding.GetBytes(footerFileContent);
            return await AddTemplateAsync(Encoding.GetBytes(fileContent), templateData, headerFileContentBytes, footerFileContentBytes);
        }

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="filePath">The path of the stylesheet file on the local filesystem.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly
        /// added stylesheet.</returns>
        public async Task<StyleFile> AddStylesheetFromFileAsync(string templateId, string filePath, StyleFile styleFileData)
        {
            using var fileContent = new StreamContent(File.OpenRead(filePath));
            var filename = Path.GetFileName(filePath);

            return await AddStylesheetAsync(templateId, fileContent, styleFileData, filename);
        }

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="fileContent">The content of the file for the new stylesheet.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly added
        /// stylesheet.</returns>
        public async Task<StyleFile> AddStylesheetAsync(string templateId, byte[] fileContent, StyleFile styleFileData)
        {
            using var fileStreamContent = new ByteArrayContent(fileContent);
            var filename = "file." + styleFileData.Format;

            return await AddStylesheetAsync(templateId, fileStreamContent, styleFileData, filename);
        }

        /// <summary>
        /// Asynchronously adds a new stylesheet to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the stylesheet will be added.</param>
        /// <param name="fileContent">The content of the file for the new stylesheet.</param>
        /// <param name="styleFileData">The meta-data of the stylesheet file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the StyleFile object of the newly added
        /// stylesheet.</returns>
        public async Task<StyleFile> AddStylesheetAsync(string templateId, string fileContent, StyleFile styleFileData)
        {
            return await AddStylesheetAsync(templateId, Encoding.GetBytes(fileContent), styleFileData);
        }

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="filePath">The path of the image file on the local filesystem.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public async Task<ImageFile> AddImageFromFileAsync(string templateId, string filePath, ImageFile imageFileData)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            var mimeType = GetFileMimeType(fileContent);
            var filename = Path.GetFileName(filePath);
            using var fileStreamContent = new ByteArrayContent(fileContent);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrEmpty(mimeType) ? "application/octet-stream" : mimeType);

            return await AddImageAsync(templateId, fileStreamContent, imageFileData, filename);
        }

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="fileContent">The content of the file for the new image.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public async Task<ImageFile> AddImageAsync(string templateId, byte[] fileContent, ImageFile imageFileData)
        {
            var mimeType = GetFileMimeType(fileContent);
            var filename = "image." + imageFileData.Format;
            using var fileStreamContent = new ByteArrayContent(fileContent);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrEmpty(mimeType) ? "application/octet-stream" : mimeType);

            return await AddImageAsync(templateId, fileStreamContent, imageFileData, filename);
        }

        /// <summary>
        /// Asynchronously adds a new image to a specified template.
        /// </summary>
        /// <param name="templateId">The ID of the template to which the image will be added.</param>
        /// <param name="fileContent">The content of the file for the new image.</param>
        /// <param name="imageFileData">The meta-data of the image file.</param>
        /// <returns>A Task representing the asynchronous operation, containing the ImageFile object of the newly added
        /// image.</returns>
        public async Task<ImageFile> AddImageAsync(string templateId, string fileContent, ImageFile imageFileData)
        {
            return await AddImageAsync(templateId, Encoding.GetBytes(fileContent), imageFileData);
        }

        /// <summary>
        /// Asynchronously deletes a template.
        /// </summary>
        /// <param name="templateId">The ID of the template to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the template was successfully
        /// deleted.</returns>
        public async Task<bool> DeleteTemplateAsync(string templateId)
        {
            return await DeleteAsync($"{_settings.BaseUrl}/template/{templateId}");
        }

        /// <summary>
        /// Asynchronously deletes a stylesheet.
        /// </summary>
        /// <param name="stylesheetId">The ID of the stylesheet to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the stylesheet was successfully
        /// deleted.</returns>
        public async Task<bool> DeleteStylesheetAsync(string stylesheetId)
        {
            return await DeleteAsync($"{_settings.BaseUrl}/template/css/{stylesheetId}");
        }

        /// <summary>
        /// Asynchronously deletes a image.
        /// </summary>
        /// <param name="imageId">The ID of the image to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, indicating whether the image was successfully
        /// deleted.</returns>
        public async Task<bool> DeleteImageAsync(string imageId)
        {
            return await DeleteAsync($"{_settings.BaseUrl}/template/img/{imageId}");
        }

        /// <summary>
        /// Asynchronously retrieves the file of a template.
        /// </summary>
        /// <param name="templateId">The ID of the template whose file is to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the template file as a
        /// byte array.</returns>
        public async Task<byte[]> GetTemplateFileAsync(string templateId)
        {
            var response = await GetAsync($"{_settings.BaseUrl}/template/file/{templateId}");
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a stylesheet.
        /// </summary>
        /// <param name="stylesheetId">The ID of the stylesheet to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the stylesheet as a byte
        /// array.</returns>
        public async Task<byte[]> GetStylesheetAsync(string stylesheetId)
        {
            var response = await GetAsync($"{_settings.BaseUrl}/template/css/file/{stylesheetId}");
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Asynchronously retrieves an image.
        /// </summary>
        /// <param name="imageId">The ID of the image to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, containing the content of the image as a byte
        /// array.</returns>
        public async Task<byte[]> GetImageAsync(string imageId)
        {
            var response = await GetAsync($"{_settings.BaseUrl}/template/img/file/{imageId}");
            return await response.Content.ReadAsByteArrayAsync();
        }

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
        public async Task<byte[]> RenderTemplateAsync(
            string templateId, 
            string renderDataPath, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            var renderDataObj = ParseRenderDataObj(renderDataPath);
            return await RenderTemplateAsync(templateId, renderDataObj, renderOptions, formatType);
        }

        /// <summary>
        /// Asynchronously renders a template with the provided render data from a file path and render options.
        /// Output format is set to PDF by default.
        /// </summary>
        /// <param name="templateId">The ID of the template to render.</param>
        /// <param name="renderData">The render data to use.</param>
        /// <param name="renderOptions">The rendering options to use.</param>
        /// <param name="formatType">The output format type  (Optional, default to pdf).</param>
        /// <returns>A Task representing the asynchronous operation, containing the byte array of the rendered template.</returns>
        public async Task<byte[]> RenderTemplateAsync(
            string templateId, 
            Dictionary<string, object> renderData, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            using var content = new MultipartFormDataContent();
            var renderDataJson = JsonConvert.SerializeObject(renderData);
            content.Add(new StringContent(renderDataJson, Encoding.UTF8, "application/json"), "render_data");

            if (renderOptions != null)
            {
                var renderOptionsJson = JsonConvert.SerializeObject(renderOptions);
                content.Add(new StringContent(renderOptionsJson, Encoding.UTF8, "application/json"), "render_options");
            }

            formatType = formatType.ToLower();
            return await PostAsync($"{_settings.BaseUrl}/render/{formatType}/{templateId}", content);
        }

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
        public async Task<byte[]> RenderTemplateManyAsync(
            string templateId, 
            string renderDataPath, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            var renderDataList = await ParseRenderDataListAsync(renderDataPath);
            
            return await RenderTemplateManyAsync(templateId, renderDataList, renderOptions, formatType);
        }

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
        public async Task<byte[]> RenderTemplateManyAsync(
            string templateId, 
            List<Dictionary<string, object>> renderDataList, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            using var content = new MultipartFormDataContent();
            var renderDataJson = JsonConvert.SerializeObject(renderDataList);
            content.Add(new StringContent(renderDataJson, Encoding.UTF8, "application/json"), "render_data");

            if (renderOptions != null)
            {
                var renderOptionsJson = JsonConvert.SerializeObject(renderOptions);
                content.Add(new StringContent(renderOptionsJson, Encoding.UTF8, "application/json"), "render_options");
            }

            formatType = formatType.ToLower();
            return await PostAsync($"{_settings.BaseUrl}/render/{formatType}/batch/{templateId}", content);
        }

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
        public async Task<byte[]> RenderFromFileAsync(
            string templateFilePath,
            Template template = null,
            object renderData = null,
            string headerFilePath = null,
            string footerFilePath = null,
            RenderOptions renderOptions = null,
            string formatType = "pdf")
        {
            // Parse render data if it's a string path
            var renderDataDict = renderData as Dictionary<string, object>
                                 ?? (renderData is string renderDataPath 
                                     ? await ParseRenderDataObjAsync(renderDataPath) 
                                     : new Dictionary<string, object>());

            // Read files as byte array content
            var templateFileBody = await ReadFileAsHttpContentAsync(templateFilePath);
            var headerFileBody = headerFilePath != null ? await ReadFileAsHttpContentAsync(headerFilePath) : null;
            var footerFileBody = footerFilePath != null ? await ReadFileAsHttpContentAsync(footerFilePath) : null;

            return await RenderInternalAsync(
                templateFileBody, 
                template, 
                renderDataDict, 
                headerFileBody, 
                footerFileBody, 
                formatType, 
                renderOptions);
        }

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
        public async Task<byte[]> RenderAsync(
            byte[] templateFileContent,
            Template template = null,
            Dictionary<string, object> renderData = null,
            byte[] headerFileContent = null,
            byte[] footerFileContent = null,
            RenderOptions renderOptions = null,
            string formatType = "pdf")
        {
            using var templateFileBody = new ByteArrayContent(templateFileContent);
            var headerFileBody = headerFileContent != null ? new ByteArrayContent(headerFileContent) : null;
            var footerFileBody = footerFileContent != null ? new ByteArrayContent(footerFileContent) : null;

            return await RenderInternalAsync(
                templateFileBody, 
                template, 
                renderData, 
                headerFileBody, 
                footerFileBody, 
                formatType, 
                renderOptions);
        }

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
        public async Task<byte[]> RenderAsync(
            string templateFileContent,
            Template template = null,
            Dictionary<string, object> renderData = null,
            string headerFileContent = null,
            string footerFileContent = null,
            RenderOptions renderOptions = null,
            string formatType = "pdf")
        {
            byte[] headerFileContentBytes = headerFileContent == null ? null : Encoding.GetBytes(headerFileContent);
            byte[] footerFileContentBytes = footerFileContent == null ? null : Encoding.GetBytes(footerFileContent);
            return await RenderAsync(
                Encoding.GetBytes(templateFileContent), 
                template, 
                renderData, 
                headerFileContentBytes, 
                footerFileContentBytes, 
                renderOptions, 
                formatType);
        }

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
        public async Task<byte[]> RenderFromFileToPDFAsync(
            string templateFilePath,
            string renderDataPath = null,
            string headerFilePath = null,
            string footerFilePath = null,
            Template template = null,
            RenderOptions renderOptions = null)
        {
            // Parse render data if it's a string path
            var renderDataDict = string.IsNullOrEmpty(renderDataPath) 
                ? new Dictionary<string, object>() 
                : await ParseRenderDataObjAsync(renderDataPath);

            // Read files as byte array content
            var templateFileBody = await ReadFileAsHttpContentAsync(templateFilePath);
            var headerFileBody = headerFilePath != null ? await ReadFileAsHttpContentAsync(headerFilePath) : null;
            var footerFileBody = footerFilePath != null ? await ReadFileAsHttpContentAsync(footerFilePath) : null;

            return await RenderInternalAsync(
                templateFileBody, 
                template, 
                renderDataDict, 
                headerFileBody, 
                footerFileBody, 
                "pdf", 
                renderOptions);
        }

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
        public async Task<byte[]> RenderToPDFAsync(
            byte[] templateFileContent,
            Dictionary<string, object> renderData = null,
            byte[] headerFileContent = null,
            byte[] footerFileContent = null,
            Template template = null,
            RenderOptions renderOptions = null)
        {
            renderData ??= new Dictionary<string, object>();
            using var templateFileBody = new ByteArrayContent(templateFileContent);
            var headerFileBody = headerFileContent != null ? new ByteArrayContent(headerFileContent) : null;
            var footerFileBody = footerFileContent != null ? new ByteArrayContent(footerFileContent) : null;

            return await RenderInternalAsync(
                templateFileBody, 
                template, 
                renderData, 
                headerFileBody, 
                footerFileBody, 
                "pdf", 
                renderOptions);
        }

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
        public async Task<byte[]> RenderToPDFAsync(
            string templateFileContent,
            Dictionary<string, object> renderData = null,
            string headerFileContent = null,
            string footerFileContent = null,
            Template template = null,
            RenderOptions renderOptions = null)
        {
            byte[] headerFileContentBytes = headerFileContent == null ? null : Encoding.GetBytes(headerFileContent);
            byte[] footerFileContentBytes = footerFileContent == null ? null : Encoding.GetBytes(footerFileContent);
            return await RenderToPDFAsync(
                Encoding.GetBytes(templateFileContent), 
                renderData, 
                headerFileContentBytes, 
                footerFileContentBytes, 
                template,
                renderOptions);
        }

        /// <summary>
        /// Asynchronously renders multiple instances of a template from its file path with provided parameters.
        /// </summary>
        /// <param name="templateFilePath">The path of the template file.</param>
        /// <param name="template">The template data, if any.</param>
        /// <param name="renderDataPath">The file path of the render data.</param>
        /// <param name="formatType">The format type for the rendered output.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered templates as a
        /// byte array.</returns>
        public async Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template template = null,
            string renderDataPath = null, 
            string formatType = "pdf")
        {
            return await RenderManyFromFileAsync(templateFilePath, template, renderDataPath, null, null, null, formatType);
        }

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
        public async Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template template = null,
            string renderDataPath = null, 
            string headerFilePath = null, 
            string footerFilePath = null,
            string formatType = "pdf")
        {
            return await RenderManyFromFileAsync(
                templateFilePath, 
                template, 
                renderDataPath, 
                headerFilePath, 
                footerFilePath, 
                null, 
                formatType);
        }

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
        public async Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template template = null,
            string renderDataPath = null, 
            string headerFilePath = null, 
            string footerFilePath = null,
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            var templateFileContent = await ReadFileAsync(templateFilePath);
            List<Dictionary<string, object>> renderDataList = null;

            if (!string.IsNullOrEmpty(renderDataPath))
            {
                renderDataList = await ParseRenderDataListAsync(renderDataPath);
            }

            byte[] headerFileContent = null;
            if (!string.IsNullOrEmpty(headerFilePath))
            {
                headerFileContent = await ReadFileAsync(headerFilePath);
            }

            byte[] footerFileContent = null;
            if (!string.IsNullOrEmpty(footerFilePath))
            {
                footerFileContent = await ReadFileAsync(footerFilePath);
            }

            return await RenderManyInternalAsync(
                templateFileContent, 
                template, 
                renderDataList,
                headerFileContent, 
                footerFileContent, 
                renderOptions, 
                formatType);
        }

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
        public async Task<byte[]> RenderManyAsync(
            byte[] templateFileContent, 
            Template template = null,
            List<Dictionary<string, object>> renderDataList = null, 
            byte[] headerFileContent = null,
            byte[] footerFileContent = null, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            return await RenderManyInternalAsync(
                templateFileContent, 
                template, 
                renderDataList,
                headerFileContent, 
                footerFileContent, 
                renderOptions, 
                formatType);
        }

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
        public async Task<byte[]> RenderManyAsync(
            string templateFileContent, 
            Template template = null,
            List<Dictionary<string, object>> renderDataList = null, 
            string headerFileContent = null,
            string footerFileContent = null, 
            RenderOptions renderOptions = null, 
            string formatType = "pdf")
        {
            byte[] headerFileContentBytes = headerFileContent == null ? null : Encoding.GetBytes(headerFileContent);
            byte[] footerFileContentBytes = footerFileContent == null ? null : Encoding.GetBytes(footerFileContent);
            return await RenderManyInternalAsync(
                Encoding.GetBytes(templateFileContent), 
                template, 
                renderDataList,
                headerFileContentBytes, 
                footerFileContentBytes, 
                renderOptions, 
                formatType);
        }

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
        public async Task<byte[]> RenderManyFromFileAsync(
            string templateFilePath, 
            Template template = null,
            List<Dictionary<string, object>> renderDataList = null, 
            string headerFilePath = null, 
            string footerFilePath = null,
            string formatType = "pdf")
        {

            var templateFileContent = await ReadFileAsync(templateFilePath);
            byte[] headerFileContent = null;
            if (!string.IsNullOrEmpty(headerFilePath))
            {
                headerFileContent = await ReadFileAsync(headerFilePath);
            }

            byte[] footerFileContent = null;
            if (!string.IsNullOrEmpty(footerFilePath))
            {
                footerFileContent = await ReadFileAsync(footerFilePath);
            }

            return await RenderManyInternalAsync(templateFileContent, template, renderDataList, 
            headerFileContent, footerFileContent, null, formatType);
        }

        /// <summary>
        /// Asynchronously renders a template to a PDF format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PDF as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToPdfAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData);
        }

        /// <summary>
        /// Asynchronously renders a template to a DOCX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered DOCX as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToDocxAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "docx");
        }

        /// <summary>
        /// Asynchronously renders a template to a ODP format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODP as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToOdpAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "odp");
        }

        /// <summary>
        /// Asynchronously renders a template to a ODS format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODS as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToOdsAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "ods");
        }

        /// <summary>
        /// Asynchronously renders a template to a ODT format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered ODT as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToOdtAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "odt");
        }

        /// <summary>
        /// Asynchronously renders a template to a PPTX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered PPTX as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToPptxAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "pptx");
        }

        /// <summary>
        /// Asynchronously renders a template to a XLX format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered XLX as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToXlxAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "xlx");
        }

        /// <summary>
        /// Asynchronously renders a template to a XLS format with provided render data.
        /// </summary>
        /// <param name="templateId">The ID of the template to be rendered.</param>
        /// <param name="renderData">The data to be rendered in the template.</param>
        /// <returns>A Task representing the asynchronous operation, containing the rendered XLS as a byte array.
        /// </returns>
        public async Task<byte[]> RenderTemplateToXlsAsync(string templateId, Dictionary<string, object> renderData)
        {
            return await RenderTemplateAsync(templateId, renderData, null, "xls");
        }
        
        private static void ConfigureJsonSerializer()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter());
        }

        private static ByteArrayContent GetPDFByteArray(byte[] data) {
            var fileByteArrayContent = new ByteArrayContent(data);
            fileByteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return fileByteArrayContent;
        }

        private async Task<HttpResponseMessage> GetAsync(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiKey);

            var response = await _httpClient.SendAsync(request);
            await RaiseForStatusAsync(response);
            return response;
        }
        
        // Helper method for GET requests that expect a JSON response
        private async Task<T> GetAsync<T>(string url)
        {
            var response = await GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        // PostAsync is a helper method for making POST requests
        private async Task<byte[]> PostAsync(string url, HttpContent content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiKey);

            var response = await _httpClient.SendAsync(request);
            await RaiseForStatusAsync(response);
            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task<string> PostAsyncString(string url, HttpContent content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiKey);

            var response = await _httpClient.SendAsync(request);
            await RaiseForStatusAsync(response);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<bool> DeleteAsync(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiKey);

            var response = await _httpClient.SendAsync(request);
            await RaiseForStatusAsync(response);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        private static async Task RaiseForStatusAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseBodyString = await response.Content.ReadAsStringAsync();
                throw new PDFException(
                    (int)response.StatusCode, 
                    response.ReasonPhrase ?? string.Empty, 
                    responseBodyString);
            }
        }

        private static HttpContent ReadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var fileContent = File.ReadAllBytes(filePath);
            return new ByteArrayContent(fileContent);
        }

        private static HttpContent ReadFile(byte[] fileContent)
        {
            if (fileContent == null)
                return null;
            return new ByteArrayContent(fileContent);
        }

        private static List<HttpContent> ReadFilesFromBytes(List<byte[]> fileContents)
        {
            var fileRequestContents = new List<HttpContent>();
            foreach (var fileContent in fileContents)
            {
                fileRequestContents.Add(ReadFile(fileContent));
            }
            return fileRequestContents;
        }

        private static List<HttpContent> ReadFiles(List<string> filePaths)
        {
            var fileRequestContents = new List<HttpContent>();
            foreach (var filePath in filePaths)
            {
                fileRequestContents.Add(ReadFile(filePath));
            }
            return fileRequestContents;
        }
        
        /// <summary>
        /// Internal method to asynchronously merge multiple PDFs represented by the given MultipartFormDataContent
        /// into a single PDF.
        /// </summary>
        /// <param name="content">Multipart form data containing the PDFs to be merged.</param>
        /// <returns>A Task representing the asynchronous operation, containing a byte array of the merged PDF.</returns>
        private async Task<byte[]> MergeAsync(HttpContent content)
        {
            return await PostAsync($"{_settings.BaseUrl}/pdf/merge", content);
        }
        
        private string GetFileMimeType(byte[] fileContent) 
        {
            var mimeType = _inspector.Inspect(fileContent).ByMimeType();
            if (mimeType.Length == 0)
                return "";
            return mimeType[0].MimeType;
        }
        
        private async Task<Template> AddTemplateAsync(
            HttpContent fileRequestBody, 
            Template templateData, 
            HttpContent headerDataBody, 
            HttpContent footerDataBody, 
            string filename, 
            string headerFilename, 
            string footerFilename)
        {
            using var content = new MultipartFormDataContent
            {
                { fileRequestBody, "file_data", filename }
            };

            if (headerDataBody != null)
            {
                content.Add(headerDataBody, "header_data", headerFilename ?? string.Empty);
            }
            if (footerDataBody != null)
            {
                content.Add(footerDataBody, "footer_data", footerFilename ?? string.Empty);
            }

            var templateJson = JsonConvert.SerializeObject(templateData);
            content.Add(new StringContent(templateJson, Encoding.UTF8, "application/json"), "template_data");

            var response = await PostAsyncString($"{_settings.BaseUrl}/template", content);
            
            return JsonConvert.DeserializeObject<Template>(response);
        }
        
        private async Task<StyleFile> AddStylesheetAsync(
            string templateId, 
            HttpContent fileRequestBody, 
            StyleFile styleFileData, 
            string filename)
        {
            using var content = new MultipartFormDataContent
            {
                { fileRequestBody, "file_data", filename }
            };
            var styleFileJson = JsonConvert.SerializeObject(styleFileData);
            content.Add(new StringContent(styleFileJson, Encoding.UTF8, "application/json"), "template_data");

            var response = await PostAsyncString($"{_settings.BaseUrl}/template/css/{templateId}", content);
            return JsonConvert.DeserializeObject<StyleFile>(response);
        }
        
        private async Task<ImageFile> AddImageAsync(
            string templateId, 
            HttpContent fileRequestBody, 
            ImageFile imageFileData, 
            string filename)
        {
            using var content = new MultipartFormDataContent
            {
                { fileRequestBody, "file_data", filename }
            };
            var imageFileJson = JsonConvert.SerializeObject(imageFileData);
            content.Add(new StringContent(imageFileJson, Encoding.UTF8, "application/json"), "template_data");

            var response = await PostAsyncString($"{_settings.BaseUrl}/template/img/{templateId}", content);
            return JsonConvert.DeserializeObject<ImageFile>(response);
        }
        
        private static Dictionary<string, object> ParseRenderDataObj(object obj)
        {
            switch (obj)
            {
                case Dictionary<string, object> dictionary:
                    return dictionary;
                case string filePath:
                {
                    var fileContent = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);
                }
                default:
                    throw new ArgumentException($"Expected Dictionary or String (file path). Unsupported render data type: {obj.GetType().Name}");
            }
        }

        private static List<Dictionary<string, object>> ParseRenderDataList(object obj)
        {
            switch (obj)
            {
                case List<Dictionary<string, object>> list:
                    return list;
                case string filePath:
                {
                    var fileContent = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fileContent);
                }
                default:
                    throw new ArgumentException("Expected List or String (file path). " +
                                                $"Unsupported render data type: {obj.GetType().Name}");
            }
        }

        private static async Task<Dictionary<string, object>> ParseRenderDataObjAsync(string path)
        {
            var fileContent = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);
        }

        private static async Task<List<Dictionary<string, object>>> ParseRenderDataListAsync(string path)
        {
            var fileContent = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fileContent);
        }

        private static async Task<byte[]> ReadFileAsync(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }

        private async Task<ByteArrayContent> ReadFileAsHttpContentAsync(string filePath)
        {
            var fileContent = await ReadFileAsync(filePath);
            return new ByteArrayContent(fileContent);
        }
        
        private async Task<byte[]> RenderInternalAsync(
            ByteArrayContent templateFileContent,
            Template template,
            Dictionary<string, object> renderData,
            ByteArrayContent headerFileContent = null,
            ByteArrayContent footerFileContent = null,
            string formatType = "pdf",
            RenderOptions renderOptions = null)
        {
            using var content = new MultipartFormDataContent
            {
                { templateFileContent, "file_data", "file.html" } // Adjust filename as needed
            };
            var renderDataJson = JsonConvert.SerializeObject(renderData);
            content.Add(new StringContent(renderDataJson, Encoding.UTF8, "application/json"), "render_data");

            template ??= new Template
            {
                Name = "fastpdf-csharp document",
                Format = "html",
                TitleHeaderEnabled = false
            };
            var templateJson = JsonConvert.SerializeObject(template);
            content.Add(new StringContent(templateJson, Encoding.UTF8, "application/json"), "template_data");

            if (headerFileContent != null)
            {
                content.Add(headerFileContent, "header_data", "header.html");
            }
            if (footerFileContent != null)
            {
                content.Add(footerFileContent, "footer_data", "footer.html");
            }
            if (renderOptions != null)
            {
                content.Add(new StringContent(JsonConvert.SerializeObject(renderOptions)), "render_options");
            }

            formatType = formatType.ToLower();
            return await PostAsync($"{_settings.BaseUrl}/render/{formatType}", content);
        }
        
        private async Task<byte[]> RenderManyInternalAsync(
            byte[] templateFileContent, 
            Template templateData,
            List<Dictionary<string, object>> renderDataList, 
            byte[] headerFileContent, 
            byte[] footerFileContent,
            RenderOptions renderOptions, 
            string formatType)
        {
            using var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(templateFileContent), "file_data", $"file.{templateData?.Format ?? "html"}" }
            };

            templateData ??= new Template
            {
                Name = "fastpdf-csharp document",
                Format = "html",
                TitleHeaderEnabled = false
            };
            var templateJson = JsonConvert.SerializeObject(templateData);
            content.Add(new StringContent(templateJson, Encoding.UTF8, "application/json"), "template_data");

            var renderDataJson = JsonConvert.SerializeObject(renderDataList ?? new List<Dictionary<string, object>>());
            content.Add(new StringContent(renderDataJson, Encoding.UTF8, "application/json"), "render_data");

            if (headerFileContent != null)
            {
                content.Add(new ByteArrayContent(headerFileContent), "header_data", "header.html");
            }

            if (footerFileContent != null)
            {
                content.Add(new ByteArrayContent(footerFileContent), "footer_data", "footer.html");
            }

            if (renderOptions != null)
            {
                var renderOptionsJson = JsonConvert.SerializeObject(renderOptions);
                content.Add(new StringContent(renderOptionsJson, Encoding.UTF8, "application/json"), "render_options");
            }

            formatType = formatType.ToLowerInvariant();
            var response = await PostAsync($"{_settings.BaseUrl}/render/{formatType}/batch", content);
            return response;
        }
    }
}
