using OfficeToPdf.API.Model.DTO;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal interface IExcelToPdfService
    {
        Task<string> ConvertToPDF(ExcelToPdfSettingsDTO settings);
    }
}
