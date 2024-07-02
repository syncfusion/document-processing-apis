using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model.DTO
{
    /// <summary>
    /// Settings to split a PDF file.
    /// </summary>
    public class SplitPdfSettingsDTO
    {
        /// <summary>
        /// Specifies the uploaded PDF document unique id.
        /// </summary>
        [Required]
        public string InputFile { get; set; }
        /// <summary>
        /// Specifies the password to unprotect the input file.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Spcify the split options
        /// </summary>
        [Required]
        public SplitOptionDTO SplitOption { get; set; }

        public string jobID { get; set; }

    }
    /// <summary>
    /// Enumerator that represents the Split Mode.
    /// </summary>
    public class SplitOptionDTO
    {
        /// <summary>
        /// Number of files to split the input file into.
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// Page count to split the input file into.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Page ranges to split the input file into.
        /// </summary>
        public List<SplitRangeDTO>? PageRanges { get; set; }
    }

    /// <summary>
    /// Split page range
    /// </summary>
    public class SplitRangeDTO
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
