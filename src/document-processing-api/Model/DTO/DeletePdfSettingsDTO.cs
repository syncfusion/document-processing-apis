using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model.DTO
{
    internal class DeletePdfSettingsDTO
    {
        internal string JobID;

        [Required]
        public string File { get; set; }

        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        public List<PageRangeDTO> PageRanges { get; set; }
    }
}
