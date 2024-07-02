using OfficeToPdf.API.Model.DTO;
namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal interface IPowerpointToPdfService
    {
        Task<string> ConvertToPDF(PowerpointToPdfSettingsDTO settings);
    }
}
