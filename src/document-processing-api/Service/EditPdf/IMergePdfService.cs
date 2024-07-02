using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal interface IMergePdfService
    {
        Task<string> MergePdf(MergePdfSettingsDTO settings);
    }
}
