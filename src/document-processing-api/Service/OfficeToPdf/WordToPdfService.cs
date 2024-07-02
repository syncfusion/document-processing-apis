using OfficeToPdf.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal class WordToPdfService : IWordToPdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public WordToPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> ConvertToPDF(WordToPdfSettingsDTO settings)
        {
            //Get input file
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.JobID);
            WordDocument wordDocument;
            //Load Word document.
            if (string.IsNullOrEmpty(settings.Password))
            {
                wordDocument = new WordDocument(docStream, Syncfusion.DocIO.FormatType.Automatic);
            }
            else
            {
                wordDocument = new WordDocument(docStream, Syncfusion.DocIO.FormatType.Automatic, settings.Password);
            }
            docStream.Dispose();

            //Instantiation of DocIORenderer for Word to PDF conversion.
            DocIORenderer render = new DocIORenderer();

            if (settings.PreserveFormFields.HasValue && settings.PreserveFormFields.Value)
            {
                render.Settings.PreserveFormFields = true;
            }
            if (!string.IsNullOrEmpty(settings.PdfComplaince))
            {
                render.Settings.PdfConformanceLevel = GetConformanceLevel(settings.PdfComplaince);
            }
            if (settings.EnableAccessibility.HasValue && settings.EnableAccessibility.Value)
            {
                render.Settings.AutoTag = true;
            }

            //Converts Word document into PDF document
            PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);

            //Dispose the resources.
            render.Dispose();
            wordDocument.Dispose();

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
