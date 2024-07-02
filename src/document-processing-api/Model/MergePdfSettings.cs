using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings to merge PDF files
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class MergePdfSettings
    {
        [Required]
        [MinLength(2)]
        public List<FileInformation> Files { get; set; }
        public bool? PreserveBookmarks { get; set; }

    }

    /// <summary>
    /// File information of the merging document.
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        /// File to merge
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Password to open the protected file
        /// </summary>
        public string? Password { get; set; }
    }


}
