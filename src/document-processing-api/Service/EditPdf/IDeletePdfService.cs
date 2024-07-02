using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal interface IDeletePdfService
    {
        Task<string> DeletePdf(DeletePdfSettingsDTO settings);
    }
}
