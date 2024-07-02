using DocumentProcessing.API.Model.DTO;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal interface IHtmlToPdfService
    {
        Task<string> ConvertHtmlToPdf(HtmlToPdfSettingsDTO settings);
    }
}
