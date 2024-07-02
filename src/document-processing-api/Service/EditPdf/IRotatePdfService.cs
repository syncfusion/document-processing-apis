using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    /// <summary>
    /// Interface for rotating PDF documents
    /// </summary>
    internal interface IRotatePdfService
    {
        /// <summary>
        /// Rotate the pages of a PDF document
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<string> RotatePdf(RotatePdfSettingsDTO settings);
    }
}
