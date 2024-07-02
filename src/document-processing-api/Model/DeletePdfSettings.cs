using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings for deleting pages from a PDF file.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class DeletePdfSettings
    {
        /// <summary>
        /// Specifies the input file to delete pages from.
        /// </summary>
        [Required]
        public string File { get; set; }
        /// <summary>
        /// Password to unprotect the input file.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Page ranges to delete the pages from PDF document.
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<PageRange> PageRanges { get; set; }
    }
}
