using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model.DTO
{
    internal class MergePdfSettingsDTO
    {
        public List<FileInformationDTO> Files { get; set; }
        public bool? PreserveBookmarks { get; set; }

        /// <summary>
        /// Job id to store the document
        /// </summary>
        public string JobID { get; set; }

    }
    internal class FileInformationDTO
    {
        public string File { get; set; }

        public string? Password { get; set; }
    }
}
