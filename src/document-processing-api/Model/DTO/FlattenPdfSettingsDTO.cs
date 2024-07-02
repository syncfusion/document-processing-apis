using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model.DTO
{
    internal class FlattenPdfSettingsDTO
    {
        /// <summary>
        /// Specifies the input file to flatten.
        /// </summary>
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
      
        public string JobID { get; set; }
    }
}
