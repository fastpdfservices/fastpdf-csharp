// -----------------------------------------------------------------
// TemplateFile.cs
// Created on 15 Nov 2023 by K.Edeline
// Description:
//   The TemplateFile class.
// -----------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FastPDFService.Models
{

    /// <summary>
    /// Represents a template file used in FastPDFService.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TemplateFile
    {
        /// <summary>
        /// Gets or sets the unique identifier of the template.
        /// </summary>
        public string Id { get; set; }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is TemplateFile file && Id == file.Id;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"TemplateFile {{ Id = {Id} }}";
        }
    }

}