using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings to split a PDF document.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class SplitPdfSettings
    {
        /// <summary>
        /// Specifies the input file to split.
        /// </summary>
        [Required]
        public string File { get; set; }
        /// <summary>
        /// Specifies the password to unprotect the input file.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Spcify the split options
        /// </summary>
        [Required]
        public SplitOption SplitOption { get; set; }

    }
    /// <summary>
    /// Enumerator that represents the Split Mode.
    /// </summary>
    public class SplitOption
    {
        /// <summary>
        /// Specifies the number of files to split the input file into.
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// Specifies the number of pages to split the input file into.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Specifies the page ranges to split the input file into.
        /// </summary>
        public List<SplitRange>? PageRanges { get; set; }
    }

    /// <summary>
    /// Split page range
    /// </summary>
    public class SplitRange
    {
        /// <summary>
        /// Start of the page range to split the input file into.
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// End of the page range to split the input file into.
        /// </summary>
        public int End { get; set; }
    }
}
