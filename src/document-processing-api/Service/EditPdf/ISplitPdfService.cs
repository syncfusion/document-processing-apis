using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal interface ISplitPdfService
    {
        Task<string> SplitPdf(SplitPdfSettingsDTO settings);
    }
}
