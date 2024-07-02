
using DocumentProcessing.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PowerpointToPdf.API.Model
{
    /// <summary>
    /// Provides settings for customizing Powerpoint to PDF conversion.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class PowerpointToPdfSettings
    {
        /// <summary>
        /// The input Powerpoint document.
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Specifies the password to open the protected input Powerpoint document.
        /// </summary>
        public string? Password { get; set; }
       
        /// <summary>
        /// Convert the power ponint document with PDF/A compliance. 
        /// Supported values are PDF/A-1B, PDF/A-2B, PDF/A-3B and PDF/A-4.
        /// </summary>
        public string? PdfComplaince { get; set; }

        /// <summary>
        /// Create accessibility tags in the PDF document.
        /// </summary>
        public bool? EnableAccessibility { get; set; }
    }
}
