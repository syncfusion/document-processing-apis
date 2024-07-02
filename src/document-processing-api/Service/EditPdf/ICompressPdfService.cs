using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal interface ICompressPdfService
    {
        Task<string> CompressPdf(CompressPdfSettingsDTO settings);
    }
}
