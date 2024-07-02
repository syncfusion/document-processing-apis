using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings to flatten a PDF document.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class FlattenPdfSettings
    {
        /// <summary>
        /// Specifies the input file to flatten.
        /// </summary>
        [Required]
        public string File { get; set; }
        /// <summary>
        /// Password to unprotect the input file.
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Specifies whether to flatten form fields.
        /// </summary>
        public bool? FlattenFormFields { get; set; }
        /// <summary>
        /// Specifies whether to flatten annotations.
        /// </summary>
        public bool? FlattenAnnotations { get; set; }

    }
}
