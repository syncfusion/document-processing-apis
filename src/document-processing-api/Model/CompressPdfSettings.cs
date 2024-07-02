using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings for compressing a PDF file.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class CompressPdfSettings
    {
        /// <summary>
        /// File to compress.
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Password to open protected file.
        /// </summary>

        public string? Password { get; set; }

        /// <summary>
        /// Specifies the image qulaity.
        /// </summary>
        public int? ImageQuality { get; set; }
        /// <summary>
        /// Optimize the font data.
        /// </summary>
        public bool? OptimizeFont { get; set; }
        /// <summary>
        /// Remove meta data informations.
        /// </summary>
        public bool? RemoveMetadata { get; set; }
        /// <summary>
        /// Optimize the page content.
        /// </summary>
        public bool? OptimizePageContents { get; set; }
        /// <summary>
        /// Flatten the form fields.
        /// </summary>
        public bool? FlattenFormFields { get; set; }
        /// <summary>
        /// Flatten the page annotations.
        /// </summary>
        public bool? FlattenAnnotations { get; set; }
    }
}
