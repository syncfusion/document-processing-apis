using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings to convert HTML to PDF.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class HtmlToPdfSettings
    {
        /// <summary>
        /// Html index file
        /// </summary>
        public string? IndexFile { get; set; }

        /// <summary>
        /// Url of the web page to convert to PDF
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// List of assets to include in the conversion
        /// </summary>
        public List<string>? Assets { get; set; }

        /// <summary>
        /// Margin to added to the PDF
        /// </summary>
        public int? Margin { get; set; }

        /// <summary>
        /// Additional delay to wait before converting the page to PDF
        /// </summary>
        public int? AdditionalDelay { get; set; }

        /// <summary>
        /// Viewport width to use when converting the page to PDF
        /// </summary>
        public int? ViewPortWidth { get; set; }
    }
}
