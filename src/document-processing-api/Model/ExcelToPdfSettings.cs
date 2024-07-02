
using DocumentProcessing.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExcelToPdf.API.Model
{
    /// <summary>
    /// Provides settings for customizing Excel to PDF conversion.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class ExcelToPdfSettings
    {
        /// <summary>
        /// The input Excel document.
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Specifies the password to open the protected input Excel document.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Convert the word document with PDF/A compliance. 
        /// Supported values are PDF/A-1B, PDF/A-2B, PDF/A-3B and PDF/A-4.
        /// </summary>
        public string? PdfComplaince { get; set; }
    }
}
