using OfficeToPdf.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal class PowerpointToPdfService : IPowerpointToPdfService
    {

        private readonly IFileStorageService _fileStorageService;
        public PowerpointToPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> ConvertToPDF(PowerpointToPdfSettingsDTO settings)
        {
            //Get input file
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.JobID);

            //Opens a PowerPoint Presentation
            IPresentation pptxDoc;
            if (string.IsNullOrEmpty(settings.Password))
                pptxDoc = Presentation.Open(docStream);
            else
                pptxDoc = Presentation.Open(docStream, settings.Password);
            docStream.Dispose();

            PresentationToPdfConverterSettings converterSettings = new PresentationToPdfConverterSettings();
            if (settings.EnableAccessibility.HasValue && settings.EnableAccessibility.Value)
            {
                converterSettings.AutoTag = true;
            }
            if (!string.IsNullOrEmpty(settings.PdfComplaince))
            {
                converterSettings.PdfConformanceLevel = GetConformanceLevel(settings.PdfComplaince);
            }
            //Converts Powerpoint document into PDF document
            PdfDocument pdfDocument = PresentationToPdfConverter.Convert(pptxDoc, converterSettings);

            //Dispose the resources.
            pptxDoc.Dispose();

            //Saves the PDF document to MemoryStream.
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            stream.Position = 0;
            //Dispose the PDF resources.
            pdfDocument.Close(true);
            PdfDocument.ClearFontCache();

            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }
        private PdfConformanceLevel GetConformanceLevel(string conformanceLevel)
        {
            switch (conformanceLevel)
            {
                case "PDF/A-1B":
                    return PdfConformanceLevel.Pdf_A1B;
                case "PDF/A-2B":
                    return PdfConformanceLevel.Pdf_A2B;
                case "PDF/A-3B":
                    return PdfConformanceLevel.Pdf_A3B;
                case "PDF/A-4":
                    return PdfConformanceLevel.Pdf_A4;
                default:
                    return PdfConformanceLevel.None;
            }
        }
    }

}
