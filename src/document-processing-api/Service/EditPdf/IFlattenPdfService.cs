using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal interface IFlattenPdfService
    {
        public Task<string> FlattenPdf(FlattenPdfSettingsDTO settings);
    }
}
