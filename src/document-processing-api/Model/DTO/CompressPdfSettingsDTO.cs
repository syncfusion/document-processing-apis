namespace DocumentProcessing.API.Model.DTO
{

    internal class CompressPdfSettingsDTO
    {
        /// <summary>
        /// File to compress.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Password to open the file.
        /// </summary>
        public string Password { get; set; }

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

        public string JobID { get; set; }
    }
}
