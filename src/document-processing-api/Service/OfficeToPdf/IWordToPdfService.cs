using OfficeToPdf.API.Model.DTO;
using WordToPdf.API.Model;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal interface IWordToPdfService
    {
        Task<string> ConvertToPDF(WordToPdfSettingsDTO settings);
    }
}
