namespace OfficeToPdf.API.Model.DTO
{
    internal class ExcelToPdfSettingsDTO
    {
        public string? InputFile { get; set; }

        /// <summary>
        /// Specifies the password to open the protected input Excel document.
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Convert the word document with PDF/A compliance. 
        /// Supported values are PDF/A-1B, PDF/A-2B, PDF/A-3B and PDF/A-4.
        /// </summary>
        public string? PdfComplaince { get; set; }

        /// <summary>
        /// Job id to store the document
        /// </summary>
        public string JobID { get; set; }
    }
}
