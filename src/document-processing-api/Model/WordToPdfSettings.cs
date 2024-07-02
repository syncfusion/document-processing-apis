
using DocumentProcessing.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WordToPdf.API.Model
{
    /// <summary>
    /// Provides settings for customizing Word to PDF conversion.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class WordToPdfSettings
    {
        /// <summary>
        /// The input Word document.
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Specifies the password to open the protected input Word document.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Convert the word document with form fields.
        /// </summary>
        public bool? PreserveFormFields { get; set; }

        /// <summary>
        /// Convert the word document with PDF/A compliance. 
        /// Supported values are PDF/A-1B, PDF/A-2B, PDF/A-3B and PDF/A-4.
        /// </summary>
        public string? PdfComplaince { get; set; }

        /// <summary>
        /// Create accessibility tags in the PDF document.
        /// </summary>
        public bool? EnableAccessibility { get; set; }

    }
   
}
